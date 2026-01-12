using System.Net;
using System.Text.Json;
using EstudoIA.Version1.Application.Shared.HttpClients.IA;

namespace EstudoIA.Version1.Application.Shared.Places;

public class WikipediaImageResolver : IPlaceImageResolver
{
    private readonly HttpClient _httpClient;

    public WikipediaImageResolver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> ResolveImageUrlAsync(string placeQuery, CancellationToken ct = default)
    {
        // Ex: "Cristo Redentor, Rio de Janeiro, Brasil"
        var rawTitle = placeQuery.Split(',')[0].Trim();

        // 1) tenta por titles direto
        var urlByTitle = BuildTitleUrl(rawTitle);
        var thumb = await GetThumbnailWithRetryAsync(urlByTitle, ct);
        if (!string.IsNullOrWhiteSpace(thumb)) return thumb;

        // 2) fallback: busca (melhor pra quando o "title" não bate certinho)
        // Use também cidade como contexto quando possível
        var searchQuery = placeQuery.Trim();
        var urlBySearch = BuildSearchUrl(searchQuery);
        thumb = await GetThumbnailWithRetryAsync(urlBySearch, ct);
        if (!string.IsNullOrWhiteSpace(thumb)) return thumb;

        // 3) fallback extra: tenta só o título “limpo” em busca
        urlBySearch = BuildSearchUrl(rawTitle);
        thumb = await GetThumbnailWithRetryAsync(urlBySearch, ct);
        if (!string.IsNullOrWhiteSpace(thumb)) return thumb;

        return null;
    }

    private static string BuildTitleUrl(string title)
    {
        return "https://pt.wikipedia.org/w/api.php" +
               "?action=query" +
               "&format=json" +
               "&origin=*" +
               "&redirects=1" +
               "&prop=pageimages" +
               "&piprop=thumbnail" +
               "&pithumbsize=1200" +
               $"&titles={Uri.EscapeDataString(title)}";
    }

    private static string BuildSearchUrl(string query)
    {
        // generator=search pega a melhor página por relevância
        return "https://pt.wikipedia.org/w/api.php" +
               "?action=query" +
               "&format=json" +
               "&origin=*" +
               "&redirects=1" +
               "&generator=search" +
               $"&gsrsearch={Uri.EscapeDataString(query)}" +
               "&gsrlimit=1" +
               "&prop=pageimages" +
               "&piprop=thumbnail" +
               "&pithumbsize=1200";
    }

    private async Task<string?> GetThumbnailWithRetryAsync(string url, CancellationToken ct)
    {
        // retries curtos e educados (Wikipedia pode rate-limit)
        var delays = new[] { 200, 600, 1500 }; // ms

        for (var attempt = 0; attempt <= delays.Length; attempt++)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                using var resp = await _httpClient.SendAsync(req, ct);

                if (resp.StatusCode == (HttpStatusCode)429 ||
                    (int)resp.StatusCode >= 500)
                {
                    // retry para rate limit / erro servidor
                    if (attempt < delays.Length)
                    {
                        await Task.Delay(delays[attempt], ct);
                        continue;
                    }

                    return null;
                }

                // Se deu 403 aqui, quase sempre é User-Agent faltando.
                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync(ct);
                return ExtractThumbnailUrl(json);
            }
            catch (OperationCanceledException) { throw; }
            catch
            {
                // timeout/rede etc.
                if (attempt < delays.Length)
                {
                    await Task.Delay(delays[attempt], ct);
                    continue;
                }

                return null;
            }
        }

        return null;
    }

    private static string? ExtractThumbnailUrl(string json)
    {
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("query", out var queryEl))
            return null;

        if (!queryEl.TryGetProperty("pages", out var pagesEl))
            return null;

        foreach (var page in pagesEl.EnumerateObject())
        {
            var pageObj = page.Value;

            if (pageObj.TryGetProperty("thumbnail", out var thumbEl) &&
                thumbEl.TryGetProperty("source", out var srcEl))
            {
                return srcEl.GetString();
            }
        }

        return null;
    }
}
