namespace EstudoIA.Version1.Application.Feature.IA.Turismo.Models;

public class TourismSummaryResponse
{
    public string City { get; init; }
    public string Country { get; init; }
    public  List<TouristSpot> Spots { get; init; } = new();
    public TourismSafetyIndexDto SafetyIndex { get; set; } = new();
    public string? SafetyNotes { get; init; }
    public string? BestTimeToVisit { get; init; }
    public string? LocalTransportTips { get; init; }
    

}

public sealed class TouristSpot
{
    public Guid Id { get; set; }

    public  string Name { get; init; }
    public string? OneLineSummary { get; init; }
    public string? WhyGo { get; init; }

    // “Como chegar” em alto nível (sem inventar detalhes específicos)
    public string? HowToGetThere { get; init; } // ex: "metrô X até estação Y + 10min a pé"
    public string? NeighborhoodOrArea { get; init; }

    // Preço como faixa/observação (evita alucinar número exato)
    public string? PriceRange { get; init; } // ex: "gratuito", "R$ 30–60", "varia por temporada"
    public string? OpeningHoursNote { get; init; } // ex: "fecha às segundas (confira no site)"
    public string? TimeNeeded { get; init; } // ex: "1–2h", "meio dia"
    public List<string> Tips { get; init; } = new();
    public string PlaceQuery { get; set; } = "";  // ex: "Parque Ibirapuera, São Paulo, Brasil"
    public string ImageUrl { get; set; } = "";    // preenchido pelo backend (Google Places)
    public string ImageSource { get; set; } = ""; // "google-places" / "confira no site oficial"
    public string Category { get; set; } = "cultura";

    public bool IsInRoute { get; set;} = false;

    public double? Lat { get; set; }
    public double? Lng { get; set; }

    public TourismSpotSafetyDto Safety { get; set; } = new();
}

public class TourismSafetyIndexDto
{
    // use string pra simplificar ("low", "medium", "high")
    public string Overall { get; set; } = "medium";
    public string Pickpocketing { get; set; } = "medium";
    public string Robbery { get; set; } = "medium";
    public string Scams { get; set; } = "medium";
    public string Notes { get; set; } = "";
    public string SourceNote { get; set; } = "Varia por bairro e horário; confira fontes oficiais e notícias locais.";
}

public class TourismSpotSafetyDto
{
    public string Overall { get; set; } = "medium";          // low|medium|high
    public string Pickpocketing { get; set; } = "medium";    // furto
    public string Robbery { get; set; } = "medium";          // roubo/assalto
    public string Scams { get; set; } = "low";               // golpes
    public string Notes { get; set; } = "";                  // dicas específicas do spot
}