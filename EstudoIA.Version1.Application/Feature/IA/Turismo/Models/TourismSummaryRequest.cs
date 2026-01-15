using System.Text.Json.Serialization;
using EstudoIA.Version1.Application.Abstractions.Handlers;

namespace EstudoIA.Version1.Application.Feature.IA.Turismo.Models;

public class TourismSummaryRequest : IRequest<TourismSummaryResponse>
{
    public required string Country { get; init; }
    public required string City { get; init; }

    // opcional: pra personalizar
    //public int MaxPlaces { get; init; } = 8;
    public string? TravelerProfile { get; init; } // ex: "família", "mochilão", "casal"
    public string? Budget { get; init; } // ex: "baixo", "médio", "alto"
    [JsonIgnore]
    public string? Language { get; init; } = "pt-BR";

    public Guid UserId { get; init; }

    public bool IsMock { get; set; }


}
