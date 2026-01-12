using System.Net.Http.Json;
using System.Text.Json;
using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;
using Microsoft.Extensions.Configuration;

namespace EstudoIA.Version1.Application.Shared.HttpClients.IA.Gemini;

public class GeminiHttpClient : IGeminiHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

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
                    ?? throw new Exception("Modelo Gemini não configurado (MachineLearning:Gemini:Model).");

        var prompt = BuildPrompt(request);

        var body = new
        {
            generationConfig = new
            {
                responseMimeType = "application/json"
            },
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(
            $"models/{model}:generateContent",
            body,
            ct);

        var rawJson = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Gemini retornou erro HTTP {(int)response.StatusCode}: {rawJson}");

        var extractedText = ExtractTextFromGeminiResponse(rawJson);
        var jsonOnly = ExtractJsonObject(extractedText);

        TourismSummaryResponse? result;
        try
        {
            result = JsonSerializer.Deserialize<TourismSummaryResponse>(
                jsonOnly,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException ex)
        {
            var preview = jsonOnly.Length <= 500 ? jsonOnly : jsonOnly.Substring(0, 500);
            throw new Exception($"Falha ao desserializar JSON do Gemini. Início do JSON: {preview}", ex);
        }

        if (result is null)
            throw new Exception("Resposta do Gemini inválida ou vazia após desserialização.");

        return result;
    }

    // ==========================
    // Prompt
    // ==========================
    private static string BuildPrompt(TourismSummaryRequest r)
    {
        return $@"
Responda APENAS com JSON VÁLIDO, sem comentários ou texto fora do JSON.

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
- Baseie-se em noções gerais de segurança turística, e deixe claro que varia por bairro e horário.
- Em ""sourceNote"", escreva: ""Varia por bairro e horário; confira fontes oficiais e notícias locais.""
- Máximo de {r.MaxPlaces} pontos turísticos.
- Idioma: {r.Language}.

Cidade: {r.City}
País: {r.Country}
Perfil do viajante: {r.TravelerProfile ?? "geral"}
Orçamento: {r.Budget ?? "geral"}
";
    }


    // ==========================
    // Helpers Gemini
    // ==========================
    private static string ExtractTextFromGeminiResponse(string rawJson)
    {
        using var doc = JsonDocument.Parse(rawJson);
        var root = doc.RootElement;

        if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            throw new Exception("Resposta do Gemini sem candidates.");

        var candidate0 = candidates[0];

        if (!candidate0.TryGetProperty("content", out var content))
            throw new Exception("Resposta do Gemini sem content em candidates[0].");

        if (!content.TryGetProperty("parts", out var parts) || parts.GetArrayLength() == 0)
            throw new Exception("Resposta do Gemini sem parts em candidates[0].content.");

        var part0 = parts[0];

        if (!part0.TryGetProperty("text", out var textEl))
            throw new Exception("Resposta do Gemini sem text em candidates[0].content.parts[0].");

        var text = textEl.GetString();

        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Texto retornado pelo Gemini vazio.");

        return text;
    }

    private static string ExtractJsonObject(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new Exception("Texto retornado pelo Gemini vazio (antes de extrair JSON).");

        text = text.Trim();

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
            var preview = text.Length <= 500 ? text : text.Substring(0, 500);
            throw new Exception($"Não foi encontrado um objeto JSON válido na resposta do Gemini. Conteúdo inicial: {preview}");
        }

        return text.Substring(start, end - start + 1).Trim();
    }
}
