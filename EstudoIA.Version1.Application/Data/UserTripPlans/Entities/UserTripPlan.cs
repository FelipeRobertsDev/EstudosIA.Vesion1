using System.Text.Json;

namespace EstudoIA.Version1.Application.Data.UserTripPlans.Entities;

public class UserTripPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string City { get; set; } = "";
    public string Country { get; set; } = "";

    public JsonDocument Route { get; set; } = default!;
    public DateTime UpdatedAt { get; set; }
}

