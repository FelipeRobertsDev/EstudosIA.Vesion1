using EstudoIA.Version1.Application.Data.Abstractions;
using System.Text.Json;

namespace EstudoIA.Version1.Application.Data.UserTripPlans;

public interface IUserTripPlansContext : IDbContextBase
{
    /// <summary>
    /// Retorna o plano de viagem (JSON) do usuário
    /// </summary>
    Task<JsonDocument?> GetRouteByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria ou atualiza o plano de viagem do usuário
    /// </summary>
    Task UpsertRouteAsync(
        Guid userId,
        string city,
        string country,
        JsonDocument route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove o plano de viagem do usuário (opcional)
    /// </summary>
    Task DeleteByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task SetSpotInRouteAsync(
        Guid userId,
        Guid spotId,
        bool isInRoute,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna o route filtrando Spots apenas com IsInRoute/isInRoute = true
    /// </summary>
    Task<JsonDocument?> GetSpotIdsInRouteAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

}
