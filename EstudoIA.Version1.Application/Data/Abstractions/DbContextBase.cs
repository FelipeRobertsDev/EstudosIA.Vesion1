using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EstudoIA.Version1.Application.Data.Abstractions
{
    public class DbContextBase : DbContext, IDbContextBase
    {

        protected readonly string _connectionString;
        public DbContextBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection DbConnection => throw new NotImplementedException();

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return base.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            Set<T>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<T?> GetByIdAsync<T>(int id, CancellationToken cancellationToken = default) where T : class
        {
            return await Set<T>().FindAsync([id], cancellationToken: cancellationToken);
        }

        public async Task<T?> GetByIdAsync<T>(string id, CancellationToken cancellationToken = default) where T : class
        {
            return await Set<T>().FindAsync([id], cancellationToken: cancellationToken);
        }

        public async Task InsertAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            await Set<T>().AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            Set<T>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task<int> WriteChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
