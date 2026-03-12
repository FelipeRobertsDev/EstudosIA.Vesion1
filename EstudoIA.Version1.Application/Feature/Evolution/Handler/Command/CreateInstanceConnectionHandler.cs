using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Evolution.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;

namespace EstudoIA.Version1.Application.Feature.Twilio.Handler.Command;

public class CreateInstanceConnectionHandler : HandlerBase<CreateInstanceConnectionRequest, CreateInstanceConnectionResponse>
{
    private readonly IWhatsAppEvolutionService _service;

    public CreateInstanceConnectionHandler(IWhatsAppEvolutionService whatsAppEvolutionService)
    {
        _service = whatsAppEvolutionService;
    }

    protected override async Task<Result<CreateInstanceConnectionResponse>> ExecuteAsync(
            CreateInstanceConnectionRequest request,
            CancellationToken cancellationToken)
    {
        var createResult = await _service.CreateInstanceAsync(
            request.InstanceName,
            cancellationToken);

        if (!createResult.Success)
        {
            return Result<CreateInstanceConnectionResponse>.Success(
                new CreateInstanceConnectionResponse
                {
                    Success = false,
                    InstanceName = request.InstanceName,
                    Error = createResult.Error
                });
        }

        var qrResult = await _service.GetQrCodeAsync(
            request.InstanceName,
            cancellationToken);

        var response = new CreateInstanceConnectionResponse
        {
            Success = qrResult.Success,
            InstanceName = request.InstanceName,
            QrCodeBase64 = qrResult.Base64,
            QrCodeText = qrResult.Code,
            Error = qrResult.Error
        };

        return Result<CreateInstanceConnectionResponse>.Success(response);
    }
    

    protected async override Task<ValidationResult> ValidateAsync(CreateInstanceConnectionRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateInstanceRequestValidator();
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
        {
            var errors = result.Errors
                .Select(e => ErrorInfo.Create(e.ErrorMessage, e.PropertyName.ToUpperInvariant()))
                .ToArray();

            return ValidationResult.Failure(errors);
        }

        return ValidationResult.Success;
    }

    public class CreateInstanceRequestValidator : AbstractValidator<CreateInstanceConnectionRequest>
    {
        public CreateInstanceRequestValidator()
        {
            RuleFor(x => x.InstanceName)
                .NotNull()
                .WithMessage("Instance name é obrigatória");
        }
    }

}
