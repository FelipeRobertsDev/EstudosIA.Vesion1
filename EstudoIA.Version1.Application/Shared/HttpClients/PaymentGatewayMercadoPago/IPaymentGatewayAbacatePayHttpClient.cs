

namespace EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;

public interface IPaymentGatewayAbacatePayHttpClient
{
    Task<PaymentResponse> ChargePaymentAsync(PaymentGatewayMercadoPagoRequest request, CancellationToken cancellationToken = default);


}
