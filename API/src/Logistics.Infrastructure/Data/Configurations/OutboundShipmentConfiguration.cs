using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class OutboundShipmentConfiguration : IEntityTypeConfiguration<OutboundShipment>
{
    public void Configure(EntityTypeBuilder<OutboundShipment> builder)
    {
        builder.ToTable("OutboundShipments");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ShipmentNumber)
            .IsRequired();

        builder.Property(s => s.OrderId)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Relacionamento com Order
        builder.HasOne(s => s.Order)
            .WithMany()
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com Vehicle - configurado em VehicleConfiguration

        builder.HasIndex(s => s.OrderId);
        builder.HasIndex(s => s.VehicleId);
    }
}
