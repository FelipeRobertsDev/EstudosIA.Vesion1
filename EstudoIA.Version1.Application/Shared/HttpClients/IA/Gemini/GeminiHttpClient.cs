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
                responseMimeType = "application/json",
                temperature = 0.6,
                maxOutputTokens = 16384
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

        // ✅ tenta reparar quando vier truncado/cortado (muito comum com listas grandes)
        jsonOnly = TryFixTruncatedJson(jsonOnly);

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
            var preview = jsonOnly.Length <= 800 ? jsonOnly : jsonOnly.Substring(0, 800);
            throw new Exception($"Falha ao desserializar JSON do Gemini (mesmo após tentativa de reparo). Início do JSON: {preview}", ex);
        }

        if (result is null)
            throw new Exception("Resposta do Gemini inválida ou vazia após desserialização.");

        foreach (var s in result.Spots)
        {
            s.Category = NormalizeCategory(s.Category);
        }

        return result;
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
            _ => "cultura" // fallback seguro
        };
    }

    // ==========================
    // Prompt
    // ==========================
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

Quantidade (OBRIGATÓRIO):
- Gere NO MÍNIMO 16 pontos turísticos e tente chegar em 20–22 (priorize consistência de JSON).
- Não repita lugares.

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

    // ==========================
    // ✅ Reparo de JSON truncado
    // ==========================
    private static string TryFixTruncatedJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        int openBraces = 0;   // {
        int openBrackets = 0; // [
        bool inString = false;
        bool escape = false;

        foreach (var ch in json)
        {
            if (inString)
            {
                if (escape) { escape = false; continue; }
                if (ch == '\\') { escape = true; continue; }
                if (ch == '"') inString = false;
                continue;
            }

            if (ch == '"') { inString = true; continue; }

            if (ch == '{') openBraces++;
            else if (ch == '}') openBraces--;
            else if (ch == '[') openBrackets++;
            else if (ch == ']') openBrackets--;
        }

        // fecha string se terminou no meio
        if (inString) json += "\"";

        // fecha arrays e objetos abertos
        if (openBrackets > 0) json += new string(']', openBrackets);
        if (openBraces > 0) json += new string('}', openBraces);

        return json;
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

        // Se por algum motivo vier com fence, remove
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
            throw new Exception($"Não foi encontrado um objeto JSON válido na resposta do Gemini. Conteúdo inicial: {preview}");
        }

        return text.Substring(start, end - start + 1).Trim();
    }
}
