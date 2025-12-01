using EstudoIA.Version1.Application.Data.PaymentContext;
using EstudoIA.Version1.Application.Data.UserContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EstudoIA.Version1.Application.Data;

public static class Module
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<IUserDbContext>(provider =>
        {
            return new UserDbContext(configuration.GetConnectionString(ConnectionsString.DefaultConnectionSqlServer) ?? string.Empty);
        });

        services.TryAddScoped<IPaymentDbContext>(provider =>
        {
            return new PaymentDbContext(configuration.GetConnectionString(ConnectionsString.DefaultConnectionSqlServer) ?? string.Empty);
        });


        return services;
    }


    public static class ConnectionsString 
    {
        public const string DefaultConnectionSqlServer = "DefaultConnectionSqlServer";
    }

}
