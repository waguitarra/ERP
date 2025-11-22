using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.PickingWave;

public record PickingWaveResponse(
    Guid Id,
    string WaveNumber,
    Guid WarehouseId,
    string WarehouseName,
    WaveStatus Status,
    DateTime? ReleasedAt,
    DateTime? CompletedAt,
    int TotalOrders,
    int TotalLines,
    DateTime CreatedAt
);
