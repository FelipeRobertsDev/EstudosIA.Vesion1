using System.Net;
using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.PaymentContext;
using EstudoIA.Version1.Application.Feature.Payment.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.PaymentGatewayMercadoPago;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;

namespace EstudoIA.Version1.Application.Feature.Payment.Handler.Command;

public class CreateChargeHandler : HandlerBase<CreatePaymentAbacatePayCommand, CreatePaymentResponse>
{

    private readonly IPaymentGatewayAbacatePayHttpClient _paymentHttpClient;
    private readonly IPaymentDbContext _paymentDbContext;

    public CreateChargeHandler(IPaymentGatewayAbacatePayHttpClient paymentGatewayAbacatePayHttp, IPaymentDbContext paymentDbContext)
    {
        _paymentHttpClient = paymentGatewayAbacatePayHttp;
        _paymentDbContext = paymentDbContext;
    }

    protected override async Task<Result<CreatePaymentResponse>> ExecuteAsync(
    CreatePaymentAbacatePayCommand request,
    CancellationToken cancellationToken)
    {
        try
        {
            var externalId = $"order_app_test_{Guid.NewGuid():N}";

            var gatewayRequest = new PaymentGatewayMercadoPagoRequest
            {
                ExternalId = externalId,
                Methods = request.Methods,
                ReturnUrl = request.ReturnUrl,
                CompletionUrl = request.CompletionUrl,
                Customer = new PaymentCustomerRequest
                {
                    Name = request.Customer.Name,
                    Email = request.Customer.Email,
                    Cellphone = request.Customer.Cellphone,
                    TaxId = request.Customer.TaxId
                },
                Products = request.Products.Select(p => new PaymentProductRequest
                {
                    ExternalId = p.ExternalId,
                    Name = p.Name,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    PriceInCents = p.PriceInCents
                }).ToList()
            };

            
            var payment = await _paymentHttpClient.ChargePaymentAsync(
                gatewayRequest,
                cancellationToken);

            
            if (payment == null ||
                string.IsNullOrWhiteSpace(payment.PaymentId) ||
                string.IsNullOrWhiteSpace(payment.CheckoutUrl) ||
                payment.AmountInCents <= 0)
            {
                return Result<CreatePaymentResponse>.Failure(
                    ErrorInfo.Create(
                        "PAYMENT_GATEWAY_INVALID_RESPONSE",
                        "Gateway retornou resposta inválida ou incompleta"),
                    statusCode: HttpStatusCode.BadGateway);
            }


            // Persiste pagamento
            var paymentEntity = new Data.PaymentContext.Entities.PaymentContext
            {
                ExternalId = externalId,
                GatewayPaymentId = payment.PaymentId,
                Gateway = "AbacatePay",
                Status = PaymentStatus.Pending,
                AmountInCents = payment.AmountInCents,
                CheckoutUrl = payment.CheckoutUrl,
                Methods = string.Join(",", payment.Methods),
                CustomerName = request.Customer.Name,
                CustomerEmail = request.Customer.Email,
                CustomerTaxId = request.Customer.TaxId,
                CustomerCellphone = request.Customer.Cellphone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _paymentDbContext.InsertAsync(paymentEntity, cancellationToken);
            await _paymentDbContext.WriteChangesAsync(cancellationToken);


            var response = new CreatePaymentResponse
            {
                PaymentId = payment.PaymentId,
                ExternalId = externalId,
                Status = payment.Status,
                AmountInCents = payment.AmountInCents,
                CheckoutUrl = payment.CheckoutUrl,
                Methods = payment.Methods,
                CreatedAt = payment.CreatedAt
            };

            return Result<CreatePaymentResponse>.Success(response);
        }
        catch (HttpRequestException ex)
        {
            return Result<CreatePaymentResponse>.Failure(
                ErrorInfo.Create(
                    "PAYMENT_GATEWAY_HTTP_ERROR",
                    ex.Message),
                statusCode: HttpStatusCode.BadGateway);
        }
        catch (Exception ex)
        {
            return Result<CreatePaymentResponse>.Failure(
                ErrorInfo.Create(
                    "PAYMENT_UNEXPECTED_ERROR",
                    ex.Message),
                statusCode: HttpStatusCode.InternalServerError);
        }
    }


    protected override async Task<ValidationResult> ValidateAsync(
      CreatePaymentAbacatePayCommand request,
      CancellationToken cancellationToken)
    {
        var validator = new CreatePaymentAbacatePayCommandValidator();
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var errors = result.Errors
                .Select(e => ErrorInfo.Create(
                    e.ErrorMessage,
                    e.PropertyName.ToUpperInvariant()))
                .ToArray();

            return ValidationResult.Failure(errors);
        }

        return ValidationResult.Success;
    }

    public sealed class CreatePaymentAbacatePayCommandValidator
    : AbstractValidator<CreatePaymentAbacatePayCommand>
    {
        public CreatePaymentAbacatePayCommandValidator()
        {
            RuleFor(x => x.ExternalId)
                .NotEmpty().WithMessage("ExternalId é obrigatório");

            RuleFor(x => x.Methods)
                .NotEmpty().WithMessage("Método de pagamento é obrigatório");

            RuleForEach(x => x.Products).SetValidator(new PaymentProductValidator());

            RuleFor(x => x.Customer).NotNull().WithMessage("Cliente é obrigatório");

            RuleFor(x => x.ReturnUrl)
                .NotEmpty().WithMessage("ReturnUrl é obrigatória");

            RuleFor(x => x.CompletionUrl)
                .NotEmpty().WithMessage("CompletionUrl é obrigatória");
        }
    }

    internal sealed class PaymentProductValidator
        : AbstractValidator<CreatePaymentProductCommand>
    {
        public PaymentProductValidator()
        {
            RuleFor(x => x.ExternalId)
                .NotEmpty().WithMessage("ExternalId do produto é obrigatório");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome do produto é obrigatório");

            RuleFor(x => x.PriceInCents)
                .GreaterThan(0).WithMessage("Preço deve ser maior que zero");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantidade deve ser maior que zero");
        }
    }

}
