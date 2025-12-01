

using EstudoIA.Version1.Application.Abstractions.Handlers;

namespace EstudoIA.Version1.Application.Abstractions.Extensions;

public static class OptionsExtensions
{
    public static void RegisterAllAssembliesFromAppDomain(this Module.Options options, string? namespacePrefix = null)
    {
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(asm =>
                !asm.IsDynamic &&
                !string.IsNullOrWhiteSpace(asm.FullName) &&
                (namespacePrefix == null || asm.FullName.StartsWith(namespacePrefix)));

        foreach (var assembly in assemblies)
        {
            options.RegisterServicesFromAssemblyContaining(assembly.GetTypes().First());
        }
    }
}
