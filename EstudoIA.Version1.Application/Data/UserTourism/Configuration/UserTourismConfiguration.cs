using EstudoIA.Version1.Application.Data.UserContext.Entities;
using EstudoIA.Version1.Application.Data.UserTourism.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstudoIA.Version1.Application.Data.UserContext.Configuration;

public class UserTourismConfiguration : IEntityTypeConfiguration<UserContextTourism>
{
    public void Configure(EntityTypeBuilder<UserContextTourism> builder)
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
