using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);
        
        // Índice único para SKU
        builder.HasIndex(p => p.SKU)
            .IsUnique();
        
        builder.Property(p => p.Barcode)
            .HasMaxLength(50);
        
        // Índice único para Barcode (quando não for null)
        builder.HasIndex(p => p.Barcode)
            .IsUnique();
        
        builder.Property(p => p.Description)
            .HasMaxLength(500);
        
        builder.Property(p => p.WeightUnit)
            .HasMaxLength(10);
        
        builder.HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
