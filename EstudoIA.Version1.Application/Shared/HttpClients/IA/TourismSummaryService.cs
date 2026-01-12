using EstudoIA.Version1.Application.Feature.IA.Turismo.Models;
using EstudoIA.Version1.Application.Shared.HttpClients.IA.Gemini;

namespace EstudoIA.Version1.Application.Shared.HttpClients.IA;

public class TourismSummaryService
{
    private readonly IGeminiHttpClient _gemini;
    private readonly IPlaceImageResolver _imageResolver;

    public TourismSummaryService(IGeminiHttpClient gemini, IPlaceImageResolver imageResolver)
    {
        _gemini = gemini;
        _imageResolver = imageResolver;
    }

    public async Task<TourismSummaryResponse> GetSummaryWithImagesAsync(
        TourismSummaryRequest request,
        CancellationToken ct = default)
    {
        var result = await _gemini.GetTourismSummaryAsync(request, ct);

        foreach (var spot in result.Spots)
        {
            var url = await _imageResolver.ResolveImageUrlAsync(spot.PlaceQuery, ct);
            spot.ImageUrl = url ?? "";
            spot.ImageSource = url is null ? "confira no site oficial" : "wikipedia";

            
            await Task.Delay(150, ct);
        }


        return result;
    }
}
