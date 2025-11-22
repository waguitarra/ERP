using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.DockDoor;

public record DockDoorResponse(
    Guid Id,
    Guid WarehouseId,
    string WarehouseName,
    string DoorNumber,
    DockDoorType Type,
    DockDoorStatus Status,
    bool IsActive,
    DateTime CreatedAt
);
