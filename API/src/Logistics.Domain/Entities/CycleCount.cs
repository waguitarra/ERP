using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class CycleCount
{
    private CycleCount() { } // EF Core

    public CycleCount(string countNumber, Guid warehouseId, Guid? zoneId, Guid countedBy)
    {
        if (string.IsNullOrWhiteSpace(countNumber)) throw new ArgumentException("CountNumber inválido");
        if (warehouseId == Guid.Empty) throw new ArgumentException("WarehouseId inválido");
        if (countedBy == Guid.Empty) throw new ArgumentException("CountedBy inválido");

        Id = Guid.NewGuid();
        CountNumber = countNumber;
        WarehouseId = warehouseId;
        ZoneId = zoneId;
        CountedBy = countedBy;
        CountDate = DateTime.UtcNow;
        Status = CycleCountStatus.InProgress;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string CountNumber { get; private set; } = string.Empty;
    public Guid WarehouseId { get; private set; }
    public Guid? ZoneId { get; private set; }
    public DateTime CountDate { get; private set; }
    public CycleCountStatus Status { get; private set; }
    public Guid CountedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Warehouse Warehouse { get; private set; } = null!;
    public WarehouseZone? Zone { get; private set; }

    public void Complete()
    {
        Status = CycleCountStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }
}
