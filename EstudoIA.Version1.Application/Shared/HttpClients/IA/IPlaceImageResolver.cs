namespace EstudoIA.Version1.Application.Shared.HttpClients.IA;

public interface IPlaceImageResolver
{
    Task<string?> ResolveImageUrlAsync(string placeQuery, CancellationToken ct = default);
}
