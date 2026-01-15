using EstudoIA.Version1.Application.Data.Abstractions;
using EstudoIA.Version1.Application.Data.UserTripPlans.Configuration;
using EstudoIA.Version1.Application.Data.UserTripPlans.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EstudoIA.Version1.Application.Data.UserTripPlans;

public class UserTripPlansDbContext : DbContextBase, IUserTripPlansContext
{
    private readonly string _connectionString;

    public DbSet<UserTripPlan> UserTripPlans { get; set; } = null!;

    // ✅ segue seu padrão: recebe string
    public UserTripPlansDbContext(string connectionString)
        : base(connectionString)
    {
        _connectionString = connectionString;
    }

    // ✅ aqui garante provider Postgres (Npgsql)
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(_connectionString);
    }

    // ✅ só um OnModelCreating (remove o duplicado)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserTripPlanConfiguration());
    }

    public async Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var plan = await UserTripPlans
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (plan is null) return;

        UserTripPlans.Remove(plan);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<JsonDocument?> GetRouteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await UserTripPlans
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Route)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpsertRouteAsync(
        Guid userId, string city, string country, JsonDocument route,
        CancellationToken cancellationToken = default)
    {
        var plan = await UserTripPlans
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (plan is null)
        {
            plan = new UserTripPlan { UserId = userId };
            await UserTripPlans.AddAsync(plan, cancellationToken);
        }

        plan.City = city;
        plan.Country = country;
        plan.Route = route;
        plan.UpdatedAt = DateTime.UtcNow;

        await SaveChangesAsync(cancellationToken);
    }
    public async Task SetSpotInRouteAsync(
    Guid userId,
    Guid spotId,
    bool isInRoute,
    CancellationToken cancellationToken = default)
    {
        var plan = await UserTripPlans
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (plan is null)
            return;

        var root = JsonNode.Parse(plan.Route.RootElement.GetRawText()) as JsonObject;
        if (root is null)
            return;

        var spotsNode = root["spots"] ?? root["Spots"];
        if (spotsNode is not JsonArray spotsArray)
            return;

        foreach (var item in spotsArray)
        {
            if (item is not JsonObject spotObj)
                continue;

            // ✅ aceita id / Id
            var idNode = spotObj["id"] ?? spotObj["Id"];
            if (idNode is null)
                continue;

            if (!Guid.TryParse(idNode.ToString(), out var currentId))
                continue;

            if (currentId == spotId)
            {
                // ✅ mantém o padrão do JSON que já existe (Pascal ou camel)
                if (spotObj.ContainsKey("IsInRoute"))
                    spotObj["IsInRoute"] = isInRoute;
                else
                    spotObj["isInRoute"] = isInRoute;

                break;
            }
        }

        plan.Route = JsonDocument.Parse(root.ToJsonString());
        plan.UpdatedAt = DateTime.UtcNow;

        await SaveChangesAsync(cancellationToken);
    }

    public async Task<JsonDocument?> GetSpotIdsInRouteAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        var route = await UserTripPlans
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Route)
            .FirstOrDefaultAsync(cancellationToken);

        if (route is null)
            return null;

        var root = JsonNode.Parse(route.RootElement.GetRawText()) as JsonObject;
        if (root is null)
            return null;

        var spotsNode = root["spots"] ?? root["Spots"];
        if (spotsNode is not JsonArray spotsArray)
            return null;

        var filteredSpots = new JsonArray();

        foreach (var item in spotsArray)
        {
            if (item is not JsonObject spotObj)
                continue;

            var inRouteNode = spotObj["isInRoute"] ?? spotObj["IsInRoute"];
            var isInRoute = false;

            if (inRouteNode is JsonValue v && v.TryGetValue<bool>(out var b))
                isInRoute = b;
            else if (inRouteNode is not null)
                bool.TryParse(inRouteNode.ToString(), out isInRoute);

            if (isInRoute)
                filteredSpots.Add(spotObj.DeepClone());
        }


        // ✅ se não tem nenhum marcado, retorna null (pra HasTrip=false)
        if (filteredSpots.Count == 0)
            return null;

        if (root.ContainsKey("spots")) root["spots"] = filteredSpots;
        else if (root.ContainsKey("Spots")) root["Spots"] = filteredSpots;
        else root["Spots"] = filteredSpots;

        return JsonDocument.Parse(root.ToJsonString());
    }



}
