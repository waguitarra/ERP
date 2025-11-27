using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class PurchaseOrderDocumentConfiguration : IEntityTypeConfiguration<PurchaseOrderDocument>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderDocument> builder)
    {
        builder.ToTable("PurchaseOrderDocuments");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

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
        builder.HasOne(d => d.PurchaseOrder)
            .WithMany(po => po.Documents)
            .HasForeignKey(d => d.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(d => d.PurchaseOrderId);
        builder.HasIndex(d => d.Type);
        builder.HasIndex(d => d.UploadedAt);
    }
}
