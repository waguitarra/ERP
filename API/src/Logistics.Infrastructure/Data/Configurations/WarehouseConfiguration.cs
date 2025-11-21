using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(w => w.Code)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.HasIndex(w => w.Code)
            .IsUnique();
        
        builder.HasOne(w => w.Company)
            .WithMany()
            .HasForeignKey(w => w.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(w => w.StorageLocations)
            .WithOne(sl => sl.Warehouse)
            .HasForeignKey(sl => sl.WarehouseId);
    }
}
