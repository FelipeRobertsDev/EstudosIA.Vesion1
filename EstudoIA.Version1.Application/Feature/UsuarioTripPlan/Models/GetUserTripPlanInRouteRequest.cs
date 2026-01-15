using EstudoIA.Version1.Application.Abstractions.Handlers;

namespace EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;

public sealed class GetUserTripPlanInRouteRequest : IRequest<GetUserTripPlanInRouteResponse>
{
    public Guid Id { get; set; }
}

