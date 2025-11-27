using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class SalesOrderItemConfiguration : IEntityTypeConfiguration<SalesOrderItem>
{
    public void Configure(EntityTypeBuilder<SalesOrderItem> builder)
    {
        builder.ToTable("SalesOrderItems");
        builder.HasKey(soi => soi.Id);

        builder.Property(soi => soi.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(soi => soi.QuantityOrdered).HasPrecision(18, 2);
        builder.Property(soi => soi.QuantityAllocated).HasPrecision(18, 2);
        builder.Property(soi => soi.QuantityPicked).HasPrecision(18, 2);
        builder.Property(soi => soi.QuantityShipped).HasPrecision(18, 2);
        builder.Property(soi => soi.UnitPrice).HasPrecision(18, 2);

        // Relationships
        builder.HasOne(soi => soi.Product)
            .WithMany()
            .HasForeignKey(soi => soi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(soi => soi.SalesOrderId);
        builder.HasIndex(soi => soi.ProductId);
        builder.HasIndex(soi => soi.SKU);
    }
}
