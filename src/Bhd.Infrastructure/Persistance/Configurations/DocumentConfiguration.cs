using Bhd.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bhd.Infrastructure.Persistance.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(d => d.Filename)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Size)
            .IsRequired();

        builder.Property(d => d.FileUrl);

        builder.Property(d => d.DocumentType)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(d => d.Channel)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(d => d.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(d => d.CorrelationId)
            .HasMaxLength(50);

        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(d => d.CorrelationId)
            .HasDatabaseName("IX_Documents_CorrelationId");

        builder.HasIndex(d => d.CustomerId)
            .HasDatabaseName("IX_Documents_CustomerId");

        builder.HasOne(d => d.User)
            .WithMany(u => u.UploadedDocuments)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
