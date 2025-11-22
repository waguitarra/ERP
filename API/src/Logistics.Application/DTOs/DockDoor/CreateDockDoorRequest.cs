using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.DockDoor;

public record CreateDockDoorRequest(
    Guid WarehouseId,
    string DoorNumber,
    DockDoorType Type
);
