using EstudoIA.Version1.Application.Services.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EstudoIA.Version1.Application.Shared.HttpClients.CloudApi
{
    public sealed class WhatsAppCloudApiService : IWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly WhatsAppCloudApiOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;

        public WhatsAppCloudApiService(
            HttpClient httpClient,
            IOptions<WhatsAppCloudApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        }

        public async Task<WhatsAppSendResult> SendTextMessageAsync(
            string to,
            string message,
            CancellationToken cancellationToken = default)
        {
            var normalizedPhone = NormalizePhone(to);

            var payload = new
            {
                messaging_product = "whatsapp",
                to = normalizedPhone,
                type = "text",
                text = new
                {
                    preview_url = false,
                    body = message
                }
            };

            return await PostMessagesAsync(payload, cancellationToken);
        }

        public async Task<WhatsAppSendResult> SendTemplateMessageAsync(
            string to,
            string templateName,
            string languageCode,
            IEnumerable<string>? bodyParameters = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedPhone = NormalizePhone(to);

            object? components = null;

            if (bodyParameters is not null)
            {
                var parameters = bodyParameters
                    .Select(x => new
                    {
                        type = "text",
                        text = x
                    })
                    .ToArray();

                if (parameters.Length > 0)
                {
                    components = new[]
                    {
                    new
                    {
                        type = "body",
                        parameters
                    }
                };
                }
            }

            var payload = new
            {
                messaging_product = "whatsapp",
                to = normalizedPhone,
                type = "template",
                template = new
                {
                    name = templateName,
                    language = new
                    {
                        code = languageCode
                    },
                    components
                }
            };

            return await PostMessagesAsync(payload, cancellationToken);
        }

        private async Task<WhatsAppSendResult> PostMessagesAsync(
            object payload,
            CancellationToken cancellationToken)
        {
            var endpoint = $"/{_options.ApiVersion}/{_options.PhoneNumberId}/messages";

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return WhatsAppSendResult.Fail(
                    $"Erro ao enviar WhatsApp. HTTP {(int)response.StatusCode}",
                    raw);
            }

            try
            {
                using var doc = JsonDocument.Parse(raw);
                var messageId = doc.RootElement
                    .GetProperty("messages")[0]
                    .GetProperty("id")
                    .GetString();

                return WhatsAppSendResult.Ok(messageId, raw);
            }
            catch
            {
                return WhatsAppSendResult.Ok(null, raw);
            }
        }

        private static string NormalizePhone(string phone)
        {
            var digits = new string(phone.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(digits))
                throw new ArgumentException("Telefone inválido.", nameof(phone));

            return digits;
        }
    }
}
