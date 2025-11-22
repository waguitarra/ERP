using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class WarehouseZoneConfiguration : IEntityTypeConfiguration<WarehouseZone>
{
    public void Configure(EntityTypeBuilder<WarehouseZone> builder)
    {
        builder.ToTable("WarehouseZones");

        builder.HasKey(z => z.Id);

        builder.Property(z => z.ZoneName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(z => z.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(z => z.Temperature)
            .HasPrecision(5, 2);

        builder.Property(z => z.Humidity)
            .HasPrecision(5, 2);

        builder.Property(z => z.TotalCapacity)
            .HasPrecision(18, 2);

        builder.Property(z => z.UsedCapacity)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(z => z.Warehouse)
            .WithMany()
            .HasForeignKey(z => z.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(z => new { z.WarehouseId, z.ZoneName })
            .IsUnique();
    }
}
