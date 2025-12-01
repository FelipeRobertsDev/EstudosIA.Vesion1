using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;

namespace EstudoIA.Version1.Application.Feature.Payment.Models;

public sealed class CreatePaymentResponse
{
    public string PaymentId { get; init; } = null!;
    public string ExternalId { get; init; } = null!;
    public PaymentStatus Status { get; init; }
    public int AmountInCents { get; init; }
    public string CheckoutUrl { get; init; } = null!;
    public IReadOnlyCollection<PaymentMethod> Methods { get; init; } = Array.Empty<PaymentMethod>();
    public DateTime CreatedAt { get; init; }
}
