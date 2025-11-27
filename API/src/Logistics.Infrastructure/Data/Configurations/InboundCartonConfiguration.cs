using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class InboundCartonConfiguration : IEntityTypeConfiguration<InboundCarton>
{
    public void Configure(EntityTypeBuilder<InboundCarton> builder)
    {
        builder.ToTable("InboundCartons");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CartonNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Barcode)
            .HasMaxLength(100);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.Weight)
            .HasPrecision(18, 2);

        builder.Property(c => c.Length)
            .HasPrecision(18, 2);

        builder.Property(c => c.Width)
            .HasPrecision(18, 2);

        builder.Property(c => c.Height)
            .HasPrecision(18, 2);

        builder.Property(c => c.DamageNotes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(c => c.InboundParcel)
            .WithMany(p => p.Cartons)
            .HasForeignKey(c => c.InboundParcelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Items)
            .WithOne(i => i.InboundCarton)
            .HasForeignKey(i => i.InboundCartonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.InboundParcelId);
        builder.HasIndex(c => c.Barcode);
        builder.HasIndex(c => c.Status);
    }
}
