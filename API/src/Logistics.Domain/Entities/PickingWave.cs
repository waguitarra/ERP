using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class PickingWave
{
    private PickingWave() { } // EF Core

    public PickingWave(string waveNumber, Guid warehouseId)
    {
        if (string.IsNullOrWhiteSpace(waveNumber)) throw new ArgumentException("WaveNumber inválido");
        if (warehouseId == Guid.Empty) throw new ArgumentException("WarehouseId inválido");

        Id = Guid.NewGuid();
        WaveNumber = waveNumber;
        WarehouseId = warehouseId;
        Status = WaveStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string WaveNumber { get; private set; } = string.Empty;
    public Guid WarehouseId { get; private set; }
    public WaveStatus Status { get; private set; }
    public DateTime? ReleasedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int TotalOrders { get; private set; }
    public int TotalLines { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Warehouse Warehouse { get; private set; } = null!;
    public ICollection<PickingTask> Tasks { get; private set; } = new List<PickingTask>();

    public void Release()
    {
        Status = WaveStatus.Released;
        ReleasedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = WaveStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTotals(int totalOrders, int totalLines)
    {
        TotalOrders = totalOrders;
        TotalLines = totalLines;
        UpdatedAt = DateTime.UtcNow;
    }
}
