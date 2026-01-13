using EstudoIA.Version1.Application.Data.Abstractions;
using EstudoIA.Version1.Application.Data.UserTourism.Entities;

namespace EstudoIA.Version1.Application.Data.UserTourism;

public interface IUserTourismContext : IDbContextBase
{
    Task<UserContextTourism?> GetByEmailAsync(string email, CancellationToken cancellationToken);


}
