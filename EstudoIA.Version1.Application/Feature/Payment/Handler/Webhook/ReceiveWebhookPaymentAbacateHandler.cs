using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.PaymentContext;
using EstudoIA.Version1.Application.Feature.Payment.Models.Webhook;
using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;
using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.Feature.Payment.Handler.Webhook;

public class ReceiveWebhookPaymentAbacateHandler
    : HandlerBase<AbacatePayWebhookRequest, Empty>
{
    private readonly IPaymentDbContext _paymentDbContext;

    public ReceiveWebhookPaymentAbacateHandler(
        IPaymentDbContext paymentDbContext)
    {
        _paymentDbContext = paymentDbContext;
    }

    protected override async Task<Result<Empty>> ExecuteAsync(
        AbacatePayWebhookRequest request,
        CancellationToken cancellationToken)
    {
        var data = request.Data;

        
        var payment = await _paymentDbContext
        .GetByGatewayPaymentIdAsync(data.Id, cancellationToken);

        if (payment == null)
        {
        
            return Result<Empty>.Success();
        }


        
        if (payment.Status == PaymentStatus.Paid)
        {
            return Result<Empty>.Success();
        }

        
        switch (data.Status.ToUpperInvariant())
        {
            case "PAID":
                payment.MarkAsPaid();
                break;

            case "CANCELLED":
                payment.MarkAsCancelled();
                break;

            case "EXPIRED":
                payment.MarkAsFailed();
                break;

            default:
                
                return Result<Empty>.Success();
        }

        payment.UpdatedAt = DateTime.UtcNow;

        await _paymentDbContext.WriteChangesAsync(cancellationToken);

        

        return Result<Empty>.Success();
    }

    protected override Task<ValidationResult> ValidateAsync(
        AbacatePayWebhookRequest request,
        CancellationToken cancellationToken)
    {
        if (request == null || request.Data == null)
        {
            return Task.FromResult(
                ValidationResult.Failure(
                    ErrorInfo.Create("WEBHOOK_INVALID_BODY", "Payload inválido")));
        }

        if (string.IsNullOrWhiteSpace(request.Data.Id))
        {
            return Task.FromResult(
                ValidationResult.Failure(
                    ErrorInfo.Create("WEBHOOK_MISSING_PAYMENT_ID", "Id do pagamento não informado")));
        }

        if (string.IsNullOrWhiteSpace(request.Data.Status))
        {
            return Task.FromResult(
                ValidationResult.Failure(
                    ErrorInfo.Create("WEBHOOK_MISSING_STATUS", "Status do pagamento não informado")));
        }

        return Task.FromResult(ValidationResult.Success);
    }
}
