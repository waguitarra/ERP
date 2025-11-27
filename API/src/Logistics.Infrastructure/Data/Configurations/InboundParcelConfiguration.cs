using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class InboundParcelConfiguration : IEntityTypeConfiguration<InboundParcel>
{
    public void Configure(EntityTypeBuilder<InboundParcel> builder)
    {
        builder.ToTable("InboundParcels");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ParcelNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.LPN)
            .HasMaxLength(100);

        builder.Property(p => p.Weight)
            .HasPrecision(18, 2);

        builder.Property(p => p.Length)
            .HasPrecision(18, 2);

        builder.Property(p => p.Width)
            .HasPrecision(18, 2);

        builder.Property(p => p.Height)
            .HasPrecision(18, 2);

        builder.Property(p => p.DimensionUnit)
            .HasMaxLength(10);

        builder.Property(p => p.CurrentLocation)
            .HasMaxLength(200);

        builder.Property(p => p.DamageNotes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(p => p.InboundShipment)
            .WithMany()
            .HasForeignKey(p => p.InboundShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ParentParcel)
            .WithMany(p => p.ChildParcels)
            .HasForeignKey(p => p.ParentParcelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Items)
            .WithOne(i => i.InboundParcel)
            .HasForeignKey(i => i.InboundParcelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Cartons)
            .WithOne(c => c.InboundParcel)
            .HasForeignKey(c => c.InboundParcelId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.InboundShipmentId);
        builder.HasIndex(p => p.LPN).IsUnique();
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.ParcelNumber);
    }
}
