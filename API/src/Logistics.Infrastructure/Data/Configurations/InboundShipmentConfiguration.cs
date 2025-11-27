using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class InboundShipmentConfiguration : IEntityTypeConfiguration<InboundShipment>
{
    public void Configure(EntityTypeBuilder<InboundShipment> builder)
    {
        builder.ToTable("InboundShipments");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ShipmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.TotalQuantityExpected)
            .HasPrecision(18, 2);

        builder.Property(s => s.TotalQuantityReceived)
            .HasPrecision(18, 2);

        builder.Property(s => s.ASNNumber)
            .HasMaxLength(100);

        builder.Property(s => s.DockDoorNumber)
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Order)
            .WithMany()
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Supplier)
            .WithMany()
            .HasForeignKey(s => s.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Vehicle)
            .WithMany()
            .HasForeignKey(s => s.VehicleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Driver)
            .WithMany()
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(s => s.ShipmentNumber).IsUnique();
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.OrderId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.ASNNumber);
    }
}
