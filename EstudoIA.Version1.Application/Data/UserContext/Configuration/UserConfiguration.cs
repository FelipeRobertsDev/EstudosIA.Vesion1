using EstudoIA.Version1.Application.Data.UserContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstudoIA.Version1.Application.Data.UserContext.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<Entities.UserContext>
{
    public void Configure(EntityTypeBuilder<Entities.UserContext> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsRequired()
            .ValueGeneratedNever(); 

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);
    }
}
