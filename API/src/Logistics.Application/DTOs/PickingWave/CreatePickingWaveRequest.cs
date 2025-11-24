namespace Logistics.Application.DTOs.PickingWave;

public class CreatePickingWaveRequest
{
    public string WaveNumber { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public List<Guid> OrderIds { get; set; } = new();
}
