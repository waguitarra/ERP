using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class VehicleMileageLogConfiguration : IEntityTypeConfiguration<VehicleMileageLog>
{
    public void Configure(EntityTypeBuilder<VehicleMileageLog> builder)
    {
        builder.ToTable("VehicleMileageLogs");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.StartMileage)
            .HasPrecision(12, 2);

        builder.Property(m => m.EndMileage)
            .HasPrecision(12, 2);

        builder.Property(m => m.StartLatitude)
            .HasPrecision(10, 7);

        builder.Property(m => m.StartLongitude)
            .HasPrecision(10, 7);

        builder.Property(m => m.EndLatitude)
            .HasPrecision(10, 7);

        builder.Property(m => m.EndLongitude)
            .HasPrecision(10, 7);

        builder.Property(m => m.StartAddress)
            .HasMaxLength(500);

        builder.Property(m => m.EndAddress)
            .HasMaxLength(500);

        builder.Property(m => m.DriverName)
            .HasMaxLength(200);

        builder.Property(m => m.ShipmentNumber)
            .HasMaxLength(50);

        builder.Property(m => m.Purpose)
            .HasMaxLength(200);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        builder.Property(m => m.RoutePolyline)
            .HasMaxLength(10000);  // Encoded polyline pode ser longo

        builder.Property(m => m.FuelConsumed)
            .HasPrecision(10, 2);

        builder.Property(m => m.FuelCost)
            .HasPrecision(12, 2);

        builder.Property(m => m.TollCost)
            .HasPrecision(12, 2);

        builder.Property(m => m.ParkingCost)
            .HasPrecision(12, 2);

        builder.Property(m => m.OtherCosts)
            .HasPrecision(12, 2);

        // Ignore computed properties
        builder.Ignore(m => m.Distance);
        builder.Ignore(m => m.Duration);
        builder.Ignore(m => m.FuelEfficiency);
        builder.Ignore(m => m.TotalCost);

        // Relationships
        builder.HasOne(m => m.Vehicle)
            .WithMany(v => v.MileageLogs)
            .HasForeignKey(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Company)
            .WithMany()
            .HasForeignKey(m => m.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Driver)
            .WithMany()
            .HasForeignKey(m => m.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Shipment)
            .WithMany()
            .HasForeignKey(m => m.ShipmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(m => m.VehicleId);
        builder.HasIndex(m => m.CompanyId);
        builder.HasIndex(m => m.DriverId);
        builder.HasIndex(m => m.ShipmentId);
        builder.HasIndex(m => m.StartDateTime);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => new { m.VehicleId, m.StartDateTime });
    }
}
