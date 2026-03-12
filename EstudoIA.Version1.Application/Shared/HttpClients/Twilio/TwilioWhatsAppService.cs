using EstudoIA.Version1.Application.Services.Options;
using EstudoIA.Version1.Application.Shared.HttpClients.Twilio;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

public sealed class TwilioWhatsAppService : IWhatsAppTwilioService
{
    private readonly HttpClient _httpClient;
    private readonly TwilioWhatsAppOptions _options;

    public TwilioWhatsAppService(
        HttpClient httpClient,
        IOptions<TwilioWhatsAppOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri("https://api.twilio.com/");
        var basic = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_options.AccountSid}:{_options.AuthToken}"));

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", basic);
    }

    public async Task<WhatsAppSendResult> SendTextMessageAsync(
        string to,
        string message,
        CancellationToken cancellationToken = default)
    {
        var toWhatsApp = NormalizeWhatsAppAddress(to);
        var fromWhatsApp = NormalizeFrom(_options.From);

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["To"] = toWhatsApp,
            ["From"] = fromWhatsApp,
            ["Body"] = message
        });

        var endpoint = $"2010-04-01/Accounts/{_options.AccountSid}/Messages.json";
        using var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return WhatsAppSendResult.Fail($"Erro Twilio HTTP {(int)response.StatusCode}", raw);
            Console.WriteLine(raw);
        var sid = TryReadJsonField(raw, "sid");
        return WhatsAppSendResult.Ok(sid, raw);
    }

    private static string NormalizeWhatsAppAddress(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits))
            throw new ArgumentException("Telefone inválido.", nameof(phone));

        return $"whatsapp:+{digits}";
    }

    private static string NormalizeFrom(string from)
    {
        if (string.IsNullOrWhiteSpace(from))
            throw new ArgumentException("From não configurado.");

        if (from.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase))
            return from;

        var digits = new string(from.Where(c => char.IsDigit(c) || c == '+').ToArray());

        if (!digits.StartsWith("+"))
            digits = "+" + new string(digits.Where(char.IsDigit).ToArray());

        return $"whatsapp:{digits}";
    }

    private static string? TryReadJsonField(string json, string fieldName)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty(fieldName, out var prop)
                ? prop.GetString()
                : null;
        }
        catch
        {
            return null;
        }
    }
}