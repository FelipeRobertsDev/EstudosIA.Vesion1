

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EstudoIA.Version1.Application.Data.Abstractions;

public interface IDbContextBase
{
    public IDbConnection DbConnection { get; }

    Task InsertAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    Task<T?> GetByIdAsync<T>(int id, CancellationToken cancellationToken = default) where T : class;
    Task<T?> GetByIdAsync<T>(string id, CancellationToken cancellationToken = default) where T : class;

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    public Task<int> WriteChangesAsync(CancellationToken cancellationToken = default);

}
