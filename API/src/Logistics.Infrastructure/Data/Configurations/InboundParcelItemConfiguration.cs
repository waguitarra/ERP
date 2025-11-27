using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class InboundParcelItemConfiguration : IEntityTypeConfiguration<InboundParcelItem>
{
    public void Configure(EntityTypeBuilder<InboundParcelItem> builder)
    {
        builder.ToTable("InboundParcelItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.ExpectedQuantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.ReceivedQuantity)
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(i => i.InboundParcel)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.InboundParcelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.InboundParcelId);
        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.SKU);
    }
}
