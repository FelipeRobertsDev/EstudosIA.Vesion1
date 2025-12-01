using EstudoIA.Version1.Application.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Reflection;

namespace EstudoIA.Version1.Application.Abstractions.Handlers;

public static class Module
{
    #region Options
    public class Options
    {
        private int _maxConcurrentBackgroundHandlersExecution = 25;
        private readonly Dictionary<string, Assembly> _assembliesToRegister = new();

        public int MaxConcurrentBackgroundHandlersExecution
        {
            get => _maxConcurrentBackgroundHandlersExecution;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "The Value must be greater than or equal to zero.");

                if (value > 100)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "The Value must be less than or equal to 100.");

                _maxConcurrentBackgroundHandlersExecution = value;
            }
        }

        public IEnumerable<Assembly> AssembliesToRegister => _assembliesToRegister.Values;

        public Options RegisterServicesFromAssemblyContaining(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            var assembly = type.Assembly;
            _assembliesToRegister[assembly.FullName] = assembly;

            return this;
        }

        public Options RegisterServicesFromAssemblyContaining<T>()
            => RegisterServicesFromAssemblyContaining(typeof(T));
    }
    #endregion

    #region Telemetry Source
    public static ActivitySource ActivitySource { get; }

    static Module()
    {
        Sdk.CreateTracerProviderBuilder()
            .AddSource("SimuladoIA.Application.Features.Abstractions")
            .SetSampler(new AlwaysOnSampler())
            .Build();

        ActivitySource = new ActivitySource("SimuladoIA.Application.Features.Abstractions");
    }
    #endregion


    #region AddHandlers
    public static IServiceCollection AddHandlers(
        this IServiceCollection services,
        Action<Options>? configureOptions = null)
    {
        // Base handler interface
        Type handlerBaseType = typeof(IHandler);

        // Configurações
        var options = new Options();
        configureOptions?.Invoke(options);

        var assemblies = options.AssembliesToRegister;
        if (!assemblies.Any())
            throw new InvalidOperationException("No assemblies were specified to register handlers.");

        // Descobrir todos os handlers
        Type[] handlers = assemblies
            .SelectMany(a => a.GetTypesSafe())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                handlerBaseType.IsAssignableFrom(t))
            .ToArray();

        foreach (var implType in handlers)
        {
            var handlerInterfaces = implType.GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.TryAddScoped(handlerInterface, implType);
            }
        }

        // Registrar coleção de handlers
        services.TryAddScoped<IHandlerCollection, HandlerCollection>();

        // Adicionar OpenTelemetry
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder.AddSource(ActivitySource.Name);
            });

        return services;
    }
    #endregion
}
