using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.QuantityOrdered)
            .HasPrecision(18, 2);

        builder.Property(i => i.QuantityAllocated)
            .HasPrecision(18, 2);

        builder.Property(i => i.QuantityPicked)
            .HasPrecision(18, 2);

        builder.Property(i => i.QuantityShipped)
            .HasPrecision(18, 2);

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(i => i.RequiredLotNumber)
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.ProductId);
    }
}
