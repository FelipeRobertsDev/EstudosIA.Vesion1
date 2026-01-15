using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudosIA.Version1.ApplicationCommon.Results;

namespace EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;

public sealed class UpdateSpotInRouteRequest : IRequest<Empty>
{
    public Guid UserId { get; set; }          // ou pega do token (se preferir)
    public Guid SpotId { get; set; }          // id do spot no JSON
    public bool IsInRoute { get; set; }       // true/false
}
