using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class OrderPriorityConfiguration : IEntityTypeConfiguration<OrderPriorityConfig>
{
    public void Configure(EntityTypeBuilder<OrderPriorityConfig> builder)
    {
        builder.ToTable("OrderPriorityConfigs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.NamePT)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NameEN)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NameES)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DescriptionPT)
            .HasMaxLength(500);

        builder.Property(e => e.DescriptionEN)
            .HasMaxLength(500);

        builder.Property(e => e.DescriptionES)
            .HasMaxLength(500);

        builder.Property(e => e.ColorHex)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasIndex(e => e.Code)
            .IsUnique();

        // Seed Data
        builder.HasData(
            new { Id = 1, Code = "LOW", NamePT = "Baixa", NameEN = "Low", NameES = "Baja", ColorHex = "#6B7280", SortOrder = 0, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 2, Code = "NORMAL", NamePT = "Normal", NameEN = "Normal", NameES = "Normal", ColorHex = "#3B82F6", SortOrder = 1, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 3, Code = "HIGH", NamePT = "Alta", NameEN = "High", NameES = "Alta", ColorHex = "#F59E0B", SortOrder = 2, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 4, Code = "URGENT", NamePT = "Urgente", NameEN = "Urgent", NameES = "Urgente", ColorHex = "#EF4444", SortOrder = 3, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
