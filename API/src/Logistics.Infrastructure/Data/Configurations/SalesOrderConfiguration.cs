using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");
        builder.HasKey(so => so.Id);

        builder.Property(so => so.SalesOrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(so => so.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(so => so.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(so => so.TotalQuantity).HasPrecision(18, 2);
        builder.Property(so => so.TotalValue).HasPrecision(18, 2);
        
        builder.Property(so => so.ShippingAddress).HasMaxLength(500);
        builder.Property(so => so.ShippingZipCode).HasMaxLength(20);
        builder.Property(so => so.ShippingCity).HasMaxLength(100);
        builder.Property(so => so.ShippingState).HasMaxLength(50);
        builder.Property(so => so.ShippingCountry).HasMaxLength(100);
        builder.Property(so => so.TrackingNumber).HasMaxLength(100);

        builder.Property(so => so.ShippingLatitude).HasPrecision(10, 7);
        builder.Property(so => so.ShippingLongitude).HasPrecision(10, 7);

        // Relationships
        builder.HasOne(so => so.Company)
            .WithMany()
            .HasForeignKey(so => so.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.Customer)
            .WithMany()
            .HasForeignKey(so => so.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(so => so.Items)
            .WithOne(i => i.SalesOrder)
            .HasForeignKey(i => i.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(so => new { so.CompanyId, so.SalesOrderNumber }).IsUnique();
        builder.HasIndex(so => so.Status);
        builder.HasIndex(so => so.OrderDate);
        builder.HasIndex(so => so.CustomerId);
        builder.HasIndex(so => so.TrackingNumber);
    }
}
