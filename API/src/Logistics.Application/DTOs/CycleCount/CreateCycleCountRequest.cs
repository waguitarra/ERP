namespace Logistics.Application.DTOs.CycleCount;

public record CreateCycleCountRequest(
    string CountNumber,
    Guid WarehouseId,
    Guid? ZoneId,
    Guid CountedBy
);
