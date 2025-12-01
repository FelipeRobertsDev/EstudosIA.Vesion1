

using EstudoIA.Version1.Application.Data.Abstractions;

namespace EstudoIA.Version1.Application.Data.UserContext;

public interface IUserDbContext : IDbContextBase
{
    Task<Entities.UserContext?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
