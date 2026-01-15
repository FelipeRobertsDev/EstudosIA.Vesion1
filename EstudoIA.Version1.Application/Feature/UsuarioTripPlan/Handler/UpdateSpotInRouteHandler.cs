using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.UserTripPlans;
using EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;
using EstudosIA.Version1.ApplicationCommon.Results;
using FluentValidation;

namespace EstudoIA.Version1.Application.UsuarioTripPlan.Handler.Command;

public sealed class UpdateSpotInRouteHandler
    : HandlerBase<UpdateSpotInRouteRequest, Empty>
{
    private readonly IUserTripPlansContext _tripPlans;

    public UpdateSpotInRouteHandler(IUserTripPlansContext tripPlans)
    {
        _tripPlans = tripPlans;
    }

    protected override async Task<Result<Empty>> ExecuteAsync(
        UpdateSpotInRouteRequest request,
        CancellationToken cancellationToken)
    {
        await _tripPlans.SetSpotInRouteAsync(
            userId: request.UserId,
            spotId: request.SpotId,
            isInRoute: request.IsInRoute,
            cancellationToken: cancellationToken);

        return Result<Empty>.Success();
    }

    protected override async Task<ValidationResult> ValidateAsync(
        UpdateSpotInRouteRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();
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

    private sealed class Validator : AbstractValidator<UpdateSpotInRouteRequest>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId é obrigatório.");

            RuleFor(x => x.SpotId)
                .NotEmpty()
                .WithMessage("SpotId é obrigatório.");
        }
    }
}
