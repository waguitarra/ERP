using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class DockDoor
{
    private DockDoor() { } // EF Core

    public DockDoor(Guid warehouseId, string doorNumber, DockDoorType type)
    {
        if (warehouseId == Guid.Empty) throw new ArgumentException("WarehouseId inválido");
        if (string.IsNullOrWhiteSpace(doorNumber)) throw new ArgumentException("DoorNumber inválido");

        Id = Guid.NewGuid();
        WarehouseId = warehouseId;
        DoorNumber = doorNumber;
        Type = type;
        Status = DockDoorStatus.Available;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string DoorNumber { get; private set; } = string.Empty;
    public DockDoorType Type { get; private set; }
    public DockDoorStatus Status { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Warehouse Warehouse { get; private set; } = null!;

    public void SetStatus(DockDoorStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        Status = DockDoorStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }
}
