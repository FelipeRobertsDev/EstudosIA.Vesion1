using EstudoIA.Version1.Application.Data.UserTripPlans.Entities;
using EstudoIA.Version1.Application.Data.UserTourism.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstudoIA.Version1.Application.Data.UserTripPlans.Configuration;

public class UserTripPlanConfiguration : IEntityTypeConfiguration<UserTripPlan>
{
    public void Configure(EntityTypeBuilder<UserTripPlan> builder)
    {
        builder.ToTable("UserTripPlan");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.City).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(120);

        builder.Property(x => x.Route).HasColumnType("jsonb").IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();
    }

}
