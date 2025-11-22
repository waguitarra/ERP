using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.CycleCount;

public record CycleCountResponse(
    Guid Id,
    string CountNumber,
    Guid WarehouseId,
    string WarehouseName,
    Guid? ZoneId,
    DateTime CountDate,
    CycleCountStatus Status,
    Guid CountedBy,
    DateTime CreatedAt
);
