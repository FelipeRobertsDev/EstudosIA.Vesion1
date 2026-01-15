
using System.Text.Json;

namespace EstudoIA.Version1.Application.Feature.UsuarioTripPlan.Models;

public class GetUserTripPlanResponse
{
    public bool HasTrip { get; set; }
    public JsonElement? Route { get; set; }
}
