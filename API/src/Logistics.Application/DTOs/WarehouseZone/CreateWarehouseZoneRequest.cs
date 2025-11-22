using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.WarehouseZone;

public record CreateWarehouseZoneRequest(
    Guid WarehouseId,
    string ZoneName,
    ZoneType Type,
    decimal? Temperature,
    decimal? Humidity,
    decimal TotalCapacity
);
