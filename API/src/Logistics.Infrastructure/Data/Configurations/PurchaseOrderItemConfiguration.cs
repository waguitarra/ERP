using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");
        builder.HasKey(poi => poi.Id);

        builder.Property(poi => poi.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(poi => poi.QuantityOrdered).HasPrecision(18, 2);
        builder.Property(poi => poi.QuantityReceived).HasPrecision(18, 2);
        builder.Property(poi => poi.UnitPrice).HasPrecision(18, 2);

        // Relationships
        builder.HasOne(poi => poi.Product)
            .WithMany()
            .HasForeignKey(poi => poi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(poi => poi.PurchaseOrderId);
        builder.HasIndex(poi => poi.ProductId);
        builder.HasIndex(poi => poi.SKU);
    }
}
