

using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Feature.Evolution.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.EvolutionApi;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;
using static EstudoIA.Version1.Application.Feature.Twilio.Handler.Command.CreateInstanceConnectionHandler;

namespace EstudoIA.Version1.Application.Feature.Evolution.Handler.Command;

public class DeleteConnectionHandler : HandlerBase<DeleteInstanceConnectionRequest, DeleteInstanceConnectionResponse>
{
    private readonly IWhatsAppEvolutionService _service;
    public DeleteConnectionHandler(IWhatsAppEvolutionService whatsAppEvolutionService)
    {
        _service = whatsAppEvolutionService;
    }
    protected override async Task<Result<DeleteInstanceConnectionResponse>> ExecuteAsync(
    DeleteInstanceConnectionRequest request,
    CancellationToken cancellationToken)
    {
        var disconnected = await _service.DisconnectAsync(
            request.InstanceName,
            cancellationToken);

        var deleted = await _service.DeleteInstanceAsync(
            request.InstanceName,
            cancellationToken);

        if (!deleted)
        {
            return Result<DeleteInstanceConnectionResponse>.Success(
                new DeleteInstanceConnectionResponse
                {
                    Success = false,
                    
                });
        }

        return Result<DeleteInstanceConnectionResponse>.Success(
            new DeleteInstanceConnectionResponse
            {
                Success = true
            });
    }

    protected async override Task<ValidationResult> ValidateAsync(DeleteInstanceConnectionRequest request, CancellationToken cancellationToken)
    {
        var validator = new DeleteConnectionRequestValidator();
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

    public class DeleteConnectionRequestValidator : AbstractValidator<DeleteInstanceConnectionRequest>
    {
        public DeleteConnectionRequestValidator()
        {
            RuleFor(x => x.InstanceName)
                .NotNull()
                .WithMessage("Instance name é obrigatória");
        }
    }




}
