using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.Feature.Payment.Models.Webhook;

public class AbacatePayWebhookRequest : IRequest<Empty>
{
    public object? Error { get; set; }
    public AbacatePayWebhookData Data { get; set; } = null!;
}

public sealed class AbacatePayWebhookData
{
    // Id da cobrança no AbacatePay (bill_xxx)
    public string Id { get; set; } = null!;

    // Seu ExternalId (order_app_test_xxx)
    public string ExternalId { get; set; } = null!;

    // Status: PENDING, PAID, CANCELLED, EXPIRED, etc
    public string Status { get; set; } = null!;

    public int Amount { get; set; }

    // Se eles mandarem a URL no webhook, ótimo:
    public string? Url { get; set; }

    public List<string>? Methods { get; set; }
}
