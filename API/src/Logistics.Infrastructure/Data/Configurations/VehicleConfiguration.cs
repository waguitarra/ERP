using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.LicensePlate)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(v => v.LicensePlate)
            .IsUnique();

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Brand)
            .HasMaxLength(50);

        builder.Property(v => v.VehicleType)
            .HasMaxLength(50);

        builder.Property(v => v.Year)
            .IsRequired();

        builder.Property(v => v.Capacity)
            .HasPrecision(10, 2);

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<int>();

        // Tracking fields
        builder.Property(v => v.TrackingToken)
            .HasMaxLength(20);

        builder.Property(v => v.TrackingEnabled)
            .HasDefaultValue(false);

        builder.Property(v => v.IsMoving)
            .HasDefaultValue(false);

        builder.Property(v => v.LastLatitude);
        builder.Property(v => v.LastLongitude);
        builder.Property(v => v.LastLocationUpdate);
        builder.Property(v => v.CurrentSpeed);

        builder.Property(v => v.CurrentAddress)
            .HasMaxLength(255);

        // Driver fields
        builder.Property(v => v.DriverId);

        builder.Property(v => v.DriverName)
            .HasMaxLength(100);

        builder.Property(v => v.DriverPhone)
            .HasMaxLength(20);

        // Current shipment
        builder.Property(v => v.CurrentShipmentId);

        // Mileage/Odometer
        builder.Property(v => v.CurrentMileage)
            .HasPrecision(12, 2)
            .HasDefaultValue(0);

        builder.Property(v => v.TotalDistanceTraveled)
            .HasPrecision(12, 2)
            .HasDefaultValue(0);

        // Financial info
        builder.Property(v => v.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(v => v.CurrentValue)
            .HasPrecision(18, 2);

        builder.Property(v => v.ChassisNumber)
            .HasMaxLength(50);

        builder.Property(v => v.EngineNumber)
            .HasMaxLength(50);

        // Maintenance costs
        builder.Property(v => v.TotalMaintenanceCost)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(v => v.LastMaintenanceMileage)
            .HasPrecision(12, 2);

        // Additional info
        builder.Property(v => v.Color)
            .HasMaxLength(30);

        builder.Property(v => v.FuelType)
            .HasMaxLength(30);

        builder.Property(v => v.Notes)
            .HasMaxLength(500);

        builder.Property(v => v.CompanyId)
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .IsRequired(false);

        // Relacionamento com Company
        builder.HasOne(v => v.Company)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com Driver
        builder.HasOne(v => v.Driver)
            .WithMany()
            .HasForeignKey(v => v.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamento com CurrentShipment (um veículo pode ter uma remessa atual)
        builder.HasOne(v => v.CurrentShipment)
            .WithOne()
            .HasForeignKey<Vehicle>(v => v.CurrentShipmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // DeliveryHistory - remessas já realizadas pelo veículo (via OutboundShipment.VehicleId)
        builder.HasMany(v => v.DeliveryHistory)
            .WithOne(s => s.Vehicle)
            .HasForeignKey(s => s.VehicleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
