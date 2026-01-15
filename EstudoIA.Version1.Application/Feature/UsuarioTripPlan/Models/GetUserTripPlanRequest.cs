using EstudoIA.Version1.Application.Abstractions.Handlers;

namespace EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;

public sealed class GetUserTripPlanRequest : IRequest<GetUserTripPlanResponse>
{
    public Guid UserId { get; set; }
}
