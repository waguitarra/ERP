using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class OrderDocumentConfiguration : IEntityTypeConfiguration<OrderDocument>
{
    public void Configure(EntityTypeBuilder<OrderDocument> builder)
    {
        builder.ToTable("OrderDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(d => d.FileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(d => d.MimeType)
            .HasMaxLength(100);

        builder.Property(d => d.DeletedAt)
            .IsRequired(false);

        builder.Property(d => d.DeletedBy)
            .IsRequired(false);

        // Relationships
        builder.HasOne(d => d.Order)
            .WithMany(o => o.Documents)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(d => d.OrderId);
        builder.HasIndex(d => d.Type);
        builder.HasIndex(d => d.UploadedAt);
    }
}
