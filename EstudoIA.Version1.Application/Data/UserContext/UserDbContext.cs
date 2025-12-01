

using EstudoIA.Version1.Application.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace EstudoIA.Version1.Application.Data.UserContext;

public class UserDbContext : DbContextBase, IUserDbContext
{
    #region Properties
    #region DbSets

    public DbSet<Entities.UserContext> Users { get; set; }
    #endregion
    #endregion

    #region Constructors
    public UserDbContext(string connectionString) : base(connectionString)
    {
    }
    #endregion
    public async Task<Entities.UserContext?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    #region Override Methods

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
    }

    

    #endregion

}
