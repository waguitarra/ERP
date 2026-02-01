using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class VehicleInspectionConfiguration : IEntityTypeConfiguration<VehicleInspection>
{
    public void Configure(EntityTypeBuilder<VehicleInspection> builder)
    {
        builder.ToTable("VehicleInspections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MileageAtInspection)
            .HasPrecision(12, 2);

        builder.Property(x => x.Cost)
            .HasPrecision(18, 2);

        builder.Property(x => x.InspectionCenter)
            .HasMaxLength(200);

        builder.Property(x => x.InspectorName)
            .HasMaxLength(100);

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(50);

        builder.Property(x => x.Observations)
            .HasMaxLength(1000);

        builder.Property(x => x.DefectsFound)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Vehicle)
            .WithMany(v => v.Inspections)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.ExpiryDate);
    }
}
