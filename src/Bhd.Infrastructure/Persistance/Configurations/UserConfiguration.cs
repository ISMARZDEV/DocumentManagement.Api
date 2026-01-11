using Bhd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bhd.Infrastructure.Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("User");

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Índices
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");
    }
}