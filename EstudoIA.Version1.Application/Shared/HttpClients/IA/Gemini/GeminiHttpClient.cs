using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EstudoIA.Version1.Application.Shared.HttpClients.IA.Gemini;

public class GeminiHttpClient : IGeminiHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private const int MIN_SPOTS = 16;
    private const int MAX_ATTEMPTS = 2;

    public GeminiHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TourismSummaryResponse> GetTourismSummaryAsync(
        TourismSummaryRequest request,
        CancellationToken ct = default)
    {
        var model = _configuration["MachineLearning:Gemini:Model"]
                    ?? throw new Exception("Modelo não configurado (MachineLearning:Gemini:Model).");

        var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("GEMINI_API_KEY não configurada");

        Console.WriteLine("BaseAddress real: " + _httpClient.BaseAddress);

        var promptBase = BuildPrompt(request);

        for (int attempt = 1; attempt <= MAX_ATTEMPTS; attempt++)
        {
            // 2ª tentativa: reforça regra e reduz temperature pra ficar mais “obediente”
            var prompt = attempt == 1
                ? promptBase
                : promptBase + @"

IMPORTANTE:
- A resposta anterior ficou INVÁLIDA.
- Gere um JSON COMPLETO e NÃO TRUNCADO.
- O array ""spots"" deve ter entre 16 e 22 itens (mínimo 16).";

            var body = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = attempt == 1 ? 0.6 : 0.2,
                max_tokens = 5000
            };

            var req = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json")
            };

            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            using var response = await _httpClient.SendAsync(req, ct);
            var rawJson = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"OpenRouter retornou erro HTTP {(int)response.StatusCode}: {rawJson}");

            var (content, finishReason) = ExtractContentAndFinishReason(rawJson);

            // Se truncou por limite de tokens, retry (se ainda tiver tentativa)
            if (string.Equals(finishReason, "length", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[IA] finish_reason=length (truncado). Tentativa {attempt}/{MAX_ATTEMPTS}");
                if (attempt == MAX_ATTEMPTS)
                    throw new Exception("Resposta truncada (finish_reason=length). Aumente max_tokens ou reduza texto do prompt.");
                continue;
            }

            string jsonOnly;
            try
            {
                jsonOnly = ExtractJsonObject(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IA] Falhou extrair JSON. Tentativa {attempt}/{MAX_ATTEMPTS}. Erro: {ex.Message}");
                if (attempt == MAX_ATTEMPTS) throw;
                continue;
            }

            TourismSummaryResponse? result;
            try
            {
                result = JsonSerializer.Deserialize<TourismSummaryResponse>(
                    jsonOnly,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[IA] JSON inválido. Tentativa {attempt}/{MAX_ATTEMPTS}. {ex.Message}");
                if (attempt == MAX_ATTEMPTS)
                {
                    var preview = jsonOnly.Length <= 800 ? jsonOnly : jsonOnly.Substring(0, 800);
                    throw new Exception($"Falha ao desserializar JSON (OpenRouter). Início do JSON: {preview}", ex);
                }
                continue;
            }

            if (result is null)
            {
                Console.WriteLine($"[IA] Result null. Tentativa {attempt}/{MAX_ATTEMPTS}");
                if (attempt == MAX_ATTEMPTS)
                    throw new Exception("Resposta inválida ou vazia após desserialização.");
                continue;
            }

            var spotsCount = result.Spots?.Count ?? 0;
            if (spotsCount < MIN_SPOTS)
            {
                Console.WriteLine($"[IA] Veio só {spotsCount} spots (<{MIN_SPOTS}). Tentativa {attempt}/{MAX_ATTEMPTS}");
                if (attempt == MAX_ATTEMPTS)
                    throw new Exception($"IA retornou apenas {spotsCount} spots (mínimo {MIN_SPOTS}).");
                continue;
            }

            foreach (var s in result.Spots)
                s.Category = NormalizeCategory(s.Category);

            return result;
        }

        throw new Exception("Falha ao gerar roteiro válido após retentativas.");
    }

    // ==========================
    // OpenRouter response parsing
    // ==========================
    private static (string content, string? finishReason) ExtractContentAndFinishReason(string rawJson)
    {
        using var doc = JsonDocument.Parse(rawJson);
        var root = doc.RootElement;

        if (!root.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            throw new Exception("Resposta do OpenRouter sem choices.");

        var choice0 = choices[0];

        string? finish = null;
        if (choice0.TryGetProperty("finish_reason", out var fr))
            finish = fr.GetString();

        if (!choice0.TryGetProperty("message", out var msg))
            throw new Exception("Resposta do OpenRouter sem message em choices[0].");

        if (!msg.TryGetProperty("content", out var contentEl))
            throw new Exception("Resposta do OpenRouter sem content em choices[0].message.");

        var text = contentEl.GetString();
        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Texto retornado pelo OpenRouter vazio.");

        return (text!, finish);
    }

    private static string NormalizeCategory(string? cat)
    {
        cat = (cat ?? "").Trim().ToLowerInvariant();

        return cat switch
        {
            "praia" or "praias" => "praias",
            "cultura" or "cultural" => "cultura",
            "gastronomia" or "comida" or "restaurantes" => "gastronomia",
            "aventura" or "natureza" or "trilha" => "aventura",
            _ => "cultura"
        };
    }

    private static string BuildPrompt(TourismSummaryRequest r)
    {
        return $@"
Responda APENAS com JSON VÁLIDO, sem comentários ou texto fora do JSON.
NÃO use markdown. NÃO use ```.

Schema:
{{
  ""city"": ""string"",
  ""country"": ""string"",
  ""bestTimeToVisit"": ""string"",
  ""localTransportTips"": ""string"",
  ""safetyNotes"": ""string"",
  ""safetyIndex"": {{
    ""overall"": ""low|medium|high"",
    ""pickpocketing"": ""low|medium|high"",
    ""robbery"": ""low|medium|high"",
    ""scams"": ""low|medium|high"",
    ""notes"": ""string"",
    ""sourceNote"": ""string""
  }},
  ""spots"": [
    {{
      ""name"": ""string"",
      ""category"": ""praias|cultura|gastronomia|aventura"",
      ""oneLineSummary"": ""string"",
      ""whyGo"": ""string"",
      ""neighborhoodOrArea"": ""string"",
      ""howToGetThere"": ""string"",
      ""priceRange"": ""string"",
      ""openingHoursNote"": ""string"",
      ""timeNeeded"": ""string"",
      ""placeQuery"": ""string"",
      ""tips"": [""string""]
    }}
  ]
}}

Regras importantes:
- Não invente números exatos de criminalidade.
- Use apenas low|medium|high para o safetyIndex.
- Baseie-se em noções gerais de segurança turística; deixe claro que varia por bairro e horário.
- Em ""sourceNote"", escreva EXATAMENTE: ""Varia por bairro e horário; confira fontes oficiais e notícias locais.""
- Idioma: {r.Language}.

Quantidade (REGRA CRÍTICA):
- Gere EXATAMENTE entre 16 e 22 itens no array ""spots"".
- Se gerar menos de 16, a resposta será considerada INVÁLIDA.
- Não encerre a resposta antes de completar no mínimo 16 itens.

Tamanho (OBRIGATÓRIO para evitar truncar):
- oneLineSummary: até 140 caracteres.
- whyGo: até 220 caracteres.
- howToGetThere: 1 linha curta.
- openingHoursNote: 1 linha curta.
- tips: no máximo 2 itens, curtos.

Regras de categorização (OBRIGATÓRIO preencher ""category""):
- Use SOMENTE: praias, cultura, gastronomia, aventura
- praias: praias, mirantes costeiros, passeios no mar, beach clubs
- cultura: museus, centros históricos, igrejas, monumentos, mirantes urbanos, tours culturais
- gastronomia: mercados, restaurantes icônicos, cafés famosos, comidas típicas
- aventura: trilhas, parques grandes, atividades ao ar livre, esportes, aquários/zoos (quando foco for experiência)

Distribuição (OBRIGATÓRIO):
- Tente distribuir assim: cultura 8–12, gastronomia 4–6, aventura 3–5, praias 0–3 (se fizer sentido).
- Se não houver praias, não invente.

Cidade: {r.City}
País: {r.Country}
Perfil do viajante: {r.TravelerProfile ?? "geral"}
Orçamento: {r.Budget ?? "geral"}
";
    }

    private static string ExtractJsonObject(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Texto retornado vazio (antes de extrair JSON).");

        text = text.Trim();

        // caso venha com fences (não deveria, mas protegemos)
        if (text.StartsWith("```"))
        {
            var firstNewLine = text.IndexOf('\n');
            if (firstNewLine >= 0)
                text = text[(firstNewLine + 1)..];

            var lastFence = text.LastIndexOf("```", StringComparison.Ordinal);
            if (lastFence >= 0)
                text = text[..lastFence];

            text = text.Trim();
        }

        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');

        if (start < 0 || end < 0 || end <= start)
        {
            var preview = text.Length <= 800 ? text : text.Substring(0, 800);
            throw new Exception($"Não foi encontrado um objeto JSON válido na resposta. Conteúdo inicial: {preview}");
        }

        return text.Substring(start, end - start + 1).Trim();
    }
}
