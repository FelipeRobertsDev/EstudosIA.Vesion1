
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EstudoIA.Version1.Application.Data.UserTripPlans.Abstractions;

public class UserTripPlansDbContextFactory : IDesignTimeDbContextFactory<UserTripPlansDbContext>
{
    public UserTripPlansDbContext CreateDbContext(string[] args)
    {
        // Caminho para a API (ajuste se necessário)
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../EstudoIA.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        // Ajuste o nome para o que você realmente tem no appsettings.json
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        return new UserTripPlansDbContext(connectionString ?? string.Empty);
    }
}
