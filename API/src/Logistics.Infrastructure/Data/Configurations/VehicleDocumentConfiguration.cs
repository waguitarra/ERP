using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class VehicleDocumentConfiguration : IEntityTypeConfiguration<VehicleDocument>
{
    public void Configure(EntityTypeBuilder<VehicleDocument> builder)
    {
        builder.ToTable("VehicleDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(300);

        builder.Property(x => x.IssuingAuthority)
            .HasMaxLength(200);

        builder.Property(x => x.FileName)
            .HasMaxLength(255);

        builder.Property(x => x.FilePath)
            .HasMaxLength(500);

        builder.Property(x => x.FileType)
            .HasMaxLength(50);

        builder.Property(x => x.Cost)
            .HasPrecision(18, 2);

        builder.Property(x => x.AlertOnExpiry)
            .HasDefaultValue(true);

        builder.Property(x => x.AlertDaysBefore)
            .HasDefaultValue(30);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Vehicle)
            .WithMany(v => v.Documents)
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
