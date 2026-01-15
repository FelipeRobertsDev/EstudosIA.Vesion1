using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.UserTripPlans;
using EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;
using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Handler;

public class UserTourismTripGetInRouteHandler : HandlerBase<GetUserTripPlanInRouteRequest, GetUserTripPlanInRouteResponse>
{

    private readonly IUserTripPlansContext _tripPlans;

    public UserTourismTripGetInRouteHandler(IUserTripPlansContext tripPlans)
    {
        _tripPlans = tripPlans;
    }

    protected override async Task<Result<GetUserTripPlanInRouteResponse>> ExecuteAsync(
        GetUserTripPlanInRouteRequest request,
        CancellationToken cancellationToken)
    {

        var userId = request.Id;

        var doc = await _tripPlans.GetSpotIdsInRouteAsync(userId, cancellationToken);

        if (doc is null)
        {
            return Result<GetUserTripPlanInRouteResponse>.Success(new GetUserTripPlanInRouteResponse
            {
                HasTrip = false,
                Route = null
            });
        }

        return Result<GetUserTripPlanInRouteResponse>.Success(new GetUserTripPlanInRouteResponse
        {
            HasTrip = true,
            Route = doc.RootElement.Clone()
        });
    }


    protected override Task<ValidationResult> ValidateAsync(GetUserTripPlanInRouteRequest request, CancellationToken cancellationToken)
        => Task.FromResult(ValidationResult.Success);

}
