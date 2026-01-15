using EstudoIA.Version1.Application.Abstractions.Handlers;
using System.Text.Json;

namespace EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;

public sealed class GetUserTripPlanInRouteResponse
{
    public bool HasTrip { get; set; }
    public JsonElement? Route { get; set; }
}
