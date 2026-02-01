using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class VehicleMaintenanceConfiguration : IEntityTypeConfiguration<VehicleMaintenance>
{
    public void Configure(EntityTypeBuilder<VehicleMaintenance> builder)
    {
        builder.ToTable("VehicleMaintenances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.MileageAtMaintenance)
            .HasPrecision(12, 2);

        builder.Property(x => x.NextMaintenanceMileage)
            .HasPrecision(12, 2);

        builder.Property(x => x.LaborCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.PartsCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.ServiceProvider)
            .HasMaxLength(200);

        builder.Property(x => x.ServiceProviderContact)
            .HasMaxLength(100);

        builder.Property(x => x.InvoiceNumber)
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Vehicle)
            .WithMany(v => v.Maintenances)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.MaintenanceDate);
    }
}
