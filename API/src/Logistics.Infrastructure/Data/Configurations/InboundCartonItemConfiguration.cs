using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class InboundCartonItemConfiguration : IEntityTypeConfiguration<InboundCartonItem>
{
    public void Configure(EntityTypeBuilder<InboundCartonItem> builder)
    {
        builder.ToTable("InboundCartonItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.SerialNumber)
            .HasMaxLength(200);

        // Relationships
        builder.HasOne(i => i.InboundCarton)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.InboundCartonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.InboundCartonId);
        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.SerialNumber);
        builder.HasIndex(i => i.SKU);
    }
}
