using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;

namespace EstudoIA.Version1.Application.Feature.Payment.Models;

public sealed class CreatePaymentAbacatePayCommand : IRequest<CreatePaymentResponse>
{
    public string ExternalId { get; init; } = null!;
    public List<CreatePaymentProductCommand> Products { get; init; } = new();
    public List<PaymentMethod> Methods { get; init; } = new();
    public CreatePaymentCustomerCommand Customer { get; init; } = null!;
    public string ReturnUrl { get; init; } = null!;
    public string CompletionUrl { get; init; } = null!;
}

public sealed class CreatePaymentProductCommand
{
    public string ExternalId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public int Quantity { get; init; }
    public int PriceInCents { get; init; }
}

public sealed class CreatePaymentCustomerCommand
{
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Cellphone { get; init; } = null!;
    public string TaxId { get; init; } = null!;
}
