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

        // Relacionamento com CurrentShipment
        builder.HasOne(v => v.CurrentShipment)
            .WithMany()
            .HasForeignKey(v => v.CurrentShipmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
