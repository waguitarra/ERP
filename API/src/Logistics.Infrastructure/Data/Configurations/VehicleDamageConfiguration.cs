using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class VehicleDamageConfiguration : IEntityTypeConfiguration<VehicleDamage>
{
    public void Configure(EntityTypeBuilder<VehicleDamage> builder)
    {
        builder.ToTable("VehicleDamages");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(d => d.DamageLocation)
            .HasMaxLength(200);

        builder.Property(d => d.DriverName)
            .HasMaxLength(200);

        builder.Property(d => d.ThirdPartyInfo)
            .HasMaxLength(500);

        builder.Property(d => d.InsuranceClaimNumber)
            .HasMaxLength(100);

        builder.Property(d => d.RepairShop)
            .HasMaxLength(200);

        builder.Property(d => d.RepairNotes)
            .HasMaxLength(1000);

        builder.Property(d => d.PhotoUrls)
            .HasMaxLength(4000);  // JSON array de URLs

        builder.Property(d => d.Notes)
            .HasMaxLength(1000);

        builder.Property(d => d.MileageAtOccurrence)
            .HasPrecision(12, 2);

        builder.Property(d => d.EstimatedRepairCost)
            .HasPrecision(12, 2);

        builder.Property(d => d.ActualRepairCost)
            .HasPrecision(12, 2);

        builder.Property(d => d.InsuranceReimbursement)
            .HasPrecision(12, 2);

        // Relationships
        builder.HasOne(d => d.Vehicle)
            .WithMany(v => v.Damages)
            .HasForeignKey(d => d.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Company)
            .WithMany()
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Driver)
            .WithMany()
            .HasForeignKey(d => d.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(d => d.VehicleId);
        builder.HasIndex(d => d.CompanyId);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.OccurrenceDate);
        builder.HasIndex(d => new { d.VehicleId, d.OccurrenceDate });
    }
}
