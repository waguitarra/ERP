using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.WarehouseZone;

public record WarehouseZoneResponse(
    Guid Id,
    Guid WarehouseId,
    string ZoneName,
    ZoneType Type,
    decimal? Temperature,
    decimal? Humidity,
    decimal TotalCapacity,
    decimal UsedCapacity,
    bool IsActive,
    DateTime CreatedAt
);
