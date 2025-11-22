using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class Package
{
    private Package() { } // EF Core

    public Package(Guid packingTaskId, string trackingNumber, PackageType type)
    {
        if (packingTaskId == Guid.Empty) throw new ArgumentException("PackingTaskId inválido");
        if (string.IsNullOrWhiteSpace(trackingNumber)) throw new ArgumentException("TrackingNumber inválido");

        Id = Guid.NewGuid();
        PackingTaskId = packingTaskId;
        TrackingNumber = trackingNumber;
        Type = type;
        Status = PackageStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PackingTaskId { get; private set; }
    public string TrackingNumber { get; private set; } = string.Empty;
    public PackageType Type { get; private set; }
    public decimal Weight { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public PackageStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public PackingTask PackingTask { get; private set; } = null!;

    public void SetDimensions(decimal weight, decimal length, decimal width, decimal height)
    {
        Weight = weight;
        Length = length;
        Width = width;
        Height = height;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(PackageStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
