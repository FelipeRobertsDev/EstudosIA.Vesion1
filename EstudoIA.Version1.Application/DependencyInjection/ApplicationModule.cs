using EstudoIA.Version1.Application.Abstractions.Extensions;
using EstudoIA.Version1.Application.Abstractions.Handlers;
using EstudoIA.Version1.Application.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && a.FullName!.StartsWith("EstudoIA"))
            .ToArray();

        var handlerType = typeof(IRequestHandler<,>);

        foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);

            foreach (var @interface in interfaces)
            {
                services.TryAddScoped(@interface, type);
            }
        }

        services.TryAddScoped<IHandlerCollection, HandlerCollection>();


        services.AddData(configuration);

        services.AddHttpClients(configuration);

        return services;
    }

}
