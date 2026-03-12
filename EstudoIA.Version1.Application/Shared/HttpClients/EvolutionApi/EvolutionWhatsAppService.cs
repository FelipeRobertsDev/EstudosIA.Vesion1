using EstudoIA.Version1.Application.Services.Options;
using EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi;
using EstudoIA.Version1.Application.Shared.HttpClients.Twilio;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

public sealed class EvolutionWhatsAppService : IWhatsAppEvolutionService
{
    private readonly HttpClient _httpClient;
    private readonly EvolutionWhatsAppOptions _options;

    public EvolutionWhatsAppService(
        HttpClient httpClient,
        IOptions<EvolutionWhatsAppOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.ServerUrl);
        _httpClient.DefaultRequestHeaders.Add("apikey", _options.ApiKey);
    }

    public async Task<WhatsAppSendResult> SendTextMessageAsync(
        string to,
        string message,
        string instanceName,
        CancellationToken cancellationToken = default)
    {
        var phone = NormalizePhone(to);

        var payload = new
        {
            number = phone,
            text = message
        };

        var endpoint = $"message/sendText/{instanceName}";

        using var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return WhatsAppSendResult.Fail($"Erro Evolution HTTP {(int)response.StatusCode}", raw);

        var messageId = TryReadMessageId(raw);

        return WhatsAppSendResult.Ok(messageId, raw);
    }

    private static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());

        if (string.IsNullOrWhiteSpace(digits))
            throw new ArgumentException("Telefone inválido.");

        return digits;
    }

    private static string? TryReadMessageId(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("key", out var key) &&
                key.TryGetProperty("id", out var id))
                return id.GetString();

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<WhatsAppInstanceCreateResult> CreateInstanceAsync(string instanceName, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            instanceName = instanceName,
            integration = "WHATSAPP-BAILEYS",
            qrcode = true
        };

        using var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            "instance/create",
            content,
            cancellationToken);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return WhatsAppInstanceCreateResult.Fail(
                $"Erro Evolution HTTP {(int)response.StatusCode}",
                raw);

        return WhatsAppInstanceCreateResult.Ok(instanceName, raw);
    }

    public async Task<WhatsAppQrCodeResult> GetQrCodeAsync(string instanceName, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
        $"instance/connect/{instanceName}",
        cancellationToken);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return WhatsAppQrCodeResult.Fail(
                $"Erro Evolution HTTP {(int)response.StatusCode}",
                raw);

        try
        {
            using var doc = JsonDocument.Parse(raw);

            var base64 = doc.RootElement
                .GetProperty("base64")
                .GetString();

            var code = doc.RootElement
                .GetProperty("code")
                .GetString();

            return WhatsAppQrCodeResult.Ok(instanceName, base64, code, raw);
        }
        catch
        {
            return WhatsAppQrCodeResult.Fail("Erro ao ler QRCode", raw);
        }
    }

    public async Task<WhatsAppConnectionStateResult> GetConnectionStateAsync(string instanceName, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
       $"instance/connectionState/{instanceName}",
       cancellationToken);

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return WhatsAppConnectionStateResult.Fail(
                $"Erro Evolution HTTP {(int)response.StatusCode}",
                raw);

        try
        {
            using var doc = JsonDocument.Parse(raw);

            var state = doc.RootElement
                .GetProperty("instance")
                .GetProperty("state")
                .GetString();

            var connected = state == "open";

            return WhatsAppConnectionStateResult.Ok(
                instanceName,
                state,
                connected,
                raw);
        }
        catch
        {
            return WhatsAppConnectionStateResult.Fail("Erro ao ler estado", raw);
        }
    }

    public async Task<bool> DisconnectAsync(string instanceName, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
       $"instance/logout/{instanceName}",
       cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteInstanceAsync(string instanceName, CancellationToken cancellationToken)
    {
        var endpoint = $"instance/delete/{instanceName}";

        var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        return response.IsSuccessStatusCode;
    }
}