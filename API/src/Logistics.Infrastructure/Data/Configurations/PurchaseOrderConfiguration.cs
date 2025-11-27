using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");
        builder.HasKey(po => po.Id);

        builder.Property(po => po.PurchaseOrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(po => po.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(po => po.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(po => po.TotalQuantity).HasPrecision(18, 2);
        builder.Property(po => po.TotalValue).HasPrecision(18, 2);
        builder.Property(po => po.UnitCost).HasPrecision(18, 2);
        builder.Property(po => po.TotalCost).HasPrecision(18, 2);
        builder.Property(po => po.TaxAmount).HasPrecision(18, 2);
        builder.Property(po => po.TaxPercentage).HasPrecision(5, 2);
        builder.Property(po => po.DesiredMarginPercentage).HasPrecision(5, 2);
        builder.Property(po => po.SuggestedSalePrice).HasPrecision(18, 2);
        builder.Property(po => po.EstimatedProfit).HasPrecision(18, 2);
        builder.Property(po => po.ShippingCost).HasPrecision(18, 2);

        builder.Property(po => po.OriginCountry).HasMaxLength(100);
        builder.Property(po => po.PortOfEntry).HasMaxLength(100);
        builder.Property(po => po.ContainerNumber).HasMaxLength(50);
        builder.Property(po => po.Incoterm).HasMaxLength(20);
        builder.Property(po => po.DockDoorNumber).HasMaxLength(50);
        builder.Property(po => po.ShippingDistance).HasMaxLength(100);

        // Relationships
        builder.HasOne(po => po.Company)
            .WithMany()
            .HasForeignKey(po => po.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(po => po.Supplier)
            .WithMany()
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(po => po.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(po => po.Documents)
            .WithOne(d => d.PurchaseOrder)
            .HasForeignKey(d => d.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(po => new { po.CompanyId, po.PurchaseOrderNumber }).IsUnique();
        builder.HasIndex(po => po.Status);
        builder.HasIndex(po => po.OrderDate);
        builder.HasIndex(po => po.SupplierId);
        builder.HasIndex(po => po.DestinationWarehouseId);
        builder.HasIndex(po => po.VehicleId);
        builder.HasIndex(po => po.DriverId);
    }
}
