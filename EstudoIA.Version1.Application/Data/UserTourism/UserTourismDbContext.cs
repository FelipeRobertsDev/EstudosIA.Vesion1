using EstudoIA.Version1.Application.Data.Abstractions;
using EstudoIA.Version1.Application.Data.UserTourism.Entities;
using Microsoft.EntityFrameworkCore;

namespace EstudoIA.Version1.Application.Data.UserTourism;

public class UserTourismDbContext : DbContextBase, IUserTourismContext
{
    #region DbSets
    public DbSet<UserContextTourism> Users { get; set; }
    #endregion

    #region Constructor
    public UserTourismDbContext(string connectionString)
        : base(connectionString)
    {
    }

    public async Task<UserContextTourism?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    #endregion

    #region Overrides
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
            _connectionString,
            o =>
            {
                o.EnableRetryOnFailure();
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            });

        }

        base.OnConfiguring(optionsBuilder);
    }

    
}


    #endregion

