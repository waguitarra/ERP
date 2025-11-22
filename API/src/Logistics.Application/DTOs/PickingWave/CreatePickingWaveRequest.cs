namespace Logistics.Application.DTOs.PickingWave;

public record CreatePickingWaveRequest(
    string WaveNumber,
    Guid WarehouseId,
    List<Guid> OrderIds
);
