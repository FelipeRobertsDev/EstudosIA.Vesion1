using EstudoIA.Version1.Application.Data.PaymentContext;
using EstudoIA.Version1.Application.Data.UserContext;
using EstudoIA.Version1.Application.Data.UserTourism;
using EstudoIA.Version1.Application.Data.UserTripPlans; // <- importante (DefaultScopes)
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace EstudoIA.Version1.Application.Data;

public static class Module
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<IUserDbContext>(_ =>
            new UserDbContext(configuration.GetConnectionString(ConnectionsString.DefaultConnectionSqlServer) ?? string.Empty));

        services.TryAddScoped<IPaymentDbContext>(_ =>
            new PaymentDbContext(configuration.GetConnectionString(ConnectionsString.DefaultConnectionSqlServer) ?? string.Empty));

        services.TryAddScoped<IUserTourismContext>(_ =>
            new UserTourismDbContext(configuration.GetConnectionString(ConnectionsString.DefaultConnectionPostgressServer) ?? string.Empty));

        services.TryAddScoped<IUserTripPlansContext>(_ =>
            new UserTripPlansDbContext(configuration.GetConnectionString(ConnectionsString.DefaultConnectionPostgressServer) ?? string.Empty));

        // ❌ REMOVER ISSO:
        // services.AddDbContext<UserTripPlansDbContext>(opt =>
        //     opt.UseNpgsql(configuration.GetConnectionString(ConnectionsString.DefaultConnectionPostgressServer)));

        return services;
    }


    public static class ConnectionsString
    {
        public const string DefaultConnectionSqlServer = "DefaultConnectionSqlServer";
        public const string DefaultConnectionPostgressServer = "DefaultConnection";
    }
}
