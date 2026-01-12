

using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;

namespace EstudoIA.Version1.Application.Shared.HttpClients.IA.Gemini;

public interface IGeminiHttpClient
{
    Task<TourismSummaryResponse> GetTourismSummaryAsync(
       TourismSummaryRequest request,
       CancellationToken ct = default);
}
