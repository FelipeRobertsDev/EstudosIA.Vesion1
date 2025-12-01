using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EstudoIA.Version1.Application.Data.UserContext.Abstractions;

internal class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        // Caminho para a API (ajuste se necessário)
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../EstudoIA.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnectionSqlServer");

        return new UserDbContext(connectionString ?? string.Empty);
    }
}
