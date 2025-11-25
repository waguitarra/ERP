using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatusConfig>
{
    public void Configure(EntityTypeBuilder<OrderStatusConfig> builder)
    {
        builder.ToTable("OrderStatusConfigs");

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
            new { Id = 1, Code = "DRAFT", NamePT = "Rascunho", NameEN = "Draft", NameES = "Borrador", ColorHex = "#6B7280", SortOrder = 0, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 2, Code = "PENDING", NamePT = "Pendente", NameEN = "Pending", NameES = "Pendiente", ColorHex = "#F59E0B", SortOrder = 1, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 3, Code = "CONFIRMED", NamePT = "Confirmado", NameEN = "Confirmed", NameES = "Confirmado", ColorHex = "#3B82F6", SortOrder = 2, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 4, Code = "IN_PROGRESS", NamePT = "Em Andamento", NameEN = "In Progress", NameES = "En Progreso", ColorHex = "#8B5CF6", SortOrder = 3, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 5, Code = "PARTIALLY_FULFILLED", NamePT = "Parcialmente Atendido", NameEN = "Partially Fulfilled", NameES = "Parcialmente Cumplido", ColorHex = "#F59E0B", SortOrder = 4, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 6, Code = "FULFILLED", NamePT = "Atendido", NameEN = "Fulfilled", NameES = "Cumplido", ColorHex = "#10B981", SortOrder = 5, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 7, Code = "SHIPPED", NamePT = "Enviado", NameEN = "Shipped", NameES = "Enviado", ColorHex = "#06B6D4", SortOrder = 6, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 8, Code = "DELIVERED", NamePT = "Entregue", NameEN = "Delivered", NameES = "Entregado", ColorHex = "#22C55E", SortOrder = 7, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 9, Code = "CANCELLED", NamePT = "Cancelado", NameEN = "Cancelled", NameES = "Cancelado", ColorHex = "#EF4444", SortOrder = 8, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 10, Code = "ON_HOLD", NamePT = "Em Espera", NameEN = "On Hold", NameES = "En Espera", ColorHex = "#F97316", SortOrder = 9, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
