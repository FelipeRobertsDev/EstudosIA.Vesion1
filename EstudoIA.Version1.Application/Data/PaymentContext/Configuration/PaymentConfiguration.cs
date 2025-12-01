using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstudoIA.Version1.Application.Data.PaymentContext.Mappings;

public class PaymentConfiguration : IEntityTypeConfiguration<Entities.PaymentContext>
{
    public void Configure(EntityTypeBuilder<Entities.PaymentContext> builder)
    {
        // =============================
        // Table
        // =============================

        builder.ToTable("Payments");

        // =============================
        // Primary Key
        // =============================

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        // =============================
        // Identifiers
        // =============================

        builder.Property(x => x.ExternalId)
            .IsRequired()
            .HasMaxLength(120);

        builder.HasIndex(x => x.ExternalId)
            .IsUnique();

        builder.Property(x => x.GatewayPaymentId)
            .IsRequired()
            .HasMaxLength(120);

        builder.HasIndex(x => x.GatewayPaymentId)
            .IsUnique();

        builder.Property(x => x.Gateway)
            .IsRequired()
            .HasMaxLength(50);

        // =============================
        // Payment
        // =============================

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.AmountInCents)
            .IsRequired();

        builder.Property(x => x.CheckoutUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Methods)
            .IsRequired()
            .HasMaxLength(100);

        // =============================
        // Customer
        // =============================

        builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.CustomerEmail)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.CustomerTaxId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CustomerCellphone)
            .IsRequired()
            .HasMaxLength(20);

        // =============================
        // Dates
        // =============================

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}
