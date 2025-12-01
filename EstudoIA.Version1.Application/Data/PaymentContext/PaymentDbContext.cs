using EstudoIA.Version1.Application.Data.Abstractions;
using EstudoIA.Version1.Application.Data.UserContext;
using Microsoft.EntityFrameworkCore;

namespace EstudoIA.Version1.Application.Data.PaymentContext
{
    public class PaymentDbContext : DbContextBase, IPaymentDbContext
    {
        #region Properties
        #region DbSets

        public DbSet<Entities.PaymentContext>  Payments { get; set; }
        #endregion
        #endregion

        #region Constructors
        public PaymentDbContext(string connectionString) : base(connectionString)
        {
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

        public async Task<Entities.PaymentContext?> GetByGatewayPaymentIdAsync(string gatewayPaymentId, CancellationToken cancellationToken = default)
        {
                return await Payments
            .FirstOrDefaultAsync(
                x => x.GatewayPaymentId == gatewayPaymentId,
                cancellationToken);
        }



        #endregion
    }


    #endregion
}