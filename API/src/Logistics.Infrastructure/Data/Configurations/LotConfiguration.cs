using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class LotConfiguration : IEntityTypeConfiguration<Lot>
{
    public void Configure(EntityTypeBuilder<Lot> builder)
    {
        builder.ToTable("Lots");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.LotNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(l => l.QuantityReceived)
            .HasPrecision(18, 2);

        builder.Property(l => l.QuantityAvailable)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(l => l.Company)
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Supplier)
            .WithMany()
            .HasForeignKey(l => l.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(l => new { l.CompanyId, l.LotNumber })
            .IsUnique();

        builder.HasIndex(l => l.ProductId);
        builder.HasIndex(l => l.ExpiryDate);
        builder.HasIndex(l => l.Status);
    }
}
