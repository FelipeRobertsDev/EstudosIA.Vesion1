using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data.UserTripPlans;
using EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;
using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.UsuarioTripPlan.Handler.Query;

public sealed class GetUserTripPlanHandler : HandlerBase<GetUserTripPlanRequest, GetUserTripPlanResponse>
{
    private readonly IUserTripPlansContext _tripPlans;

    public GetUserTripPlanHandler(IUserTripPlansContext tripPlans)
    {
        _tripPlans = tripPlans;
    }

    protected override async Task<Result<GetUserTripPlanResponse>> ExecuteAsync(
        GetUserTripPlanRequest request,
        CancellationToken cancellationToken)
    {
        
        var userId = request.UserId;

        
        var doc = await _tripPlans.GetRouteByUserIdAsync(userId, cancellationToken);

        if (doc is null)
        {
            return Result<GetUserTripPlanResponse>.Success(new GetUserTripPlanResponse
            {
                HasTrip = false,
                Route = null
            });
        }

        
        return Result<GetUserTripPlanResponse>.Success(new GetUserTripPlanResponse
        {
            HasTrip = true,
            Route = doc.RootElement.Clone()
        });
    }

    
    protected override Task<ValidationResult> ValidateAsync(GetUserTripPlanRequest request, CancellationToken cancellationToken)
        => Task.FromResult(ValidationResult.Success);

    
 
}
