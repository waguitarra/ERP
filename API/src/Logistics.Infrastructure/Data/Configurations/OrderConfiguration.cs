using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.Source)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.Priority)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.TotalQuantity)
            .HasPrecision(18, 2);

        builder.Property(o => o.TotalValue)
            .HasPrecision(18, 2);

        builder.Property(o => o.ShippingAddress)
            .HasMaxLength(500);

        builder.Property(o => o.SpecialInstructions)
            .HasMaxLength(1000);

        // WMS Fields - Geolocalização
        builder.Property(o => o.ShippingZipCode)
            .HasMaxLength(20);

        builder.Property(o => o.ShippingLatitude)
            .HasPrecision(10, 8);

        builder.Property(o => o.ShippingLongitude)
            .HasPrecision(11, 8);

        builder.Property(o => o.ShippingCity)
            .HasMaxLength(100);

        builder.Property(o => o.ShippingState)
            .HasMaxLength(50);

        builder.Property(o => o.ShippingCountry)
            .HasMaxLength(50);

        // WMS Fields - Rastreamento
        builder.Property(o => o.TrackingNumber)
            .HasMaxLength(100);

        // Purchase Order - Preços
        builder.Property(o => o.UnitCost)
            .HasPrecision(18, 2);

        builder.Property(o => o.TotalCost)
            .HasPrecision(18, 2);

        builder.Property(o => o.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.TaxPercentage)
            .HasPrecision(5, 2);

        builder.Property(o => o.DesiredMarginPercentage)
            .HasPrecision(5, 2);

        builder.Property(o => o.SuggestedSalePrice)
            .HasPrecision(18, 2);

        builder.Property(o => o.EstimatedProfit)
            .HasPrecision(18, 2);

        builder.Property(o => o.ShippingCost)
            .HasPrecision(18, 2);

        // Purchase Order - Logística
        builder.Property(o => o.ShippingDistance)
            .HasMaxLength(50);

        builder.Property(o => o.DockDoorNumber)
            .HasMaxLength(50);

        // Purchase Order - Internacional
        builder.Property(o => o.OriginCountry)
            .HasMaxLength(100);

        builder.Property(o => o.PortOfEntry)
            .HasMaxLength(100);

        builder.Property(o => o.CustomsBroker)
            .HasMaxLength(200);

        builder.Property(o => o.ThirdPartyCarrier)
            .HasMaxLength(200);

        builder.Property(o => o.ContainerNumber)
            .HasMaxLength(50);

        builder.Property(o => o.BillOfLading)
            .HasMaxLength(100);

        builder.Property(o => o.ImportLicenseNumber)
            .HasMaxLength(100);

        builder.Property(o => o.Incoterm)
            .HasMaxLength(10);

        // Relationships
        builder.HasOne(o => o.Company)
            .WithMany()
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Supplier)
            .WithMany()
            .HasForeignKey(o => o.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Documents)
            .WithOne(d => d.Order)
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // WMS Relationships
        builder.HasOne(o => o.Vehicle)
            .WithMany()
            .HasForeignKey(o => o.VehicleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.Driver)
            .WithMany()
            .HasForeignKey(o => o.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.OriginWarehouse)
            .WithMany()
            .HasForeignKey(o => o.OriginWarehouseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.DestinationWarehouse)
            .WithMany()
            .HasForeignKey(o => o.DestinationWarehouseId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(o => new { o.CompanyId, o.OrderNumber })
            .IsUnique();

        builder.HasIndex(o => o.OrderDate);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.VehicleId);
        builder.HasIndex(o => o.DriverId);
        builder.HasIndex(o => o.OriginWarehouseId);
        builder.HasIndex(o => o.DestinationWarehouseId);
        builder.HasIndex(o => o.TrackingNumber);
        builder.HasIndex(o => o.ContainerNumber);
        builder.HasIndex(o => o.IsInternational);
    }
}
