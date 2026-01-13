using EstudoIA.Version1.Application.Data.UserTourism;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EstudoIA.Version1.Application.Data.UserContext.Abstractions;

internal class UserDbTourismContextFactory : IDesignTimeDbContextFactory<UserTourismDbContext>
{
    public UserTourismDbContext CreateDbContext(string[] args)
    {
        // Caminho para a API (ajuste se necessário)
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../EstudoIA.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        // Ajuste o nome para o que você realmente tem no appsettings.json
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        return new UserTourismDbContext(connectionString ?? string.Empty);
    }
}
