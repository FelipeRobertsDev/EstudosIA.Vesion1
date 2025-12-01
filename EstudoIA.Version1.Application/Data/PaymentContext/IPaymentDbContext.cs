

using EstudoIA.Version1.Application.Data.Abstractions;

namespace EstudoIA.Version1.Application.Data.PaymentContext;

public interface IPaymentDbContext : IDbContextBase
{
    Task<Entities.PaymentContext?> GetByGatewayPaymentIdAsync(
    string gatewayPaymentId,
    CancellationToken cancellationToken = default);

}
