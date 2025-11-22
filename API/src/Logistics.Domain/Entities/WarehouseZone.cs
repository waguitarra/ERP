using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class WarehouseZone
{
    private WarehouseZone() { } // EF Core

    public WarehouseZone(Guid warehouseId, string zoneName, ZoneType type)
    {
        if (warehouseId == Guid.Empty)
            throw new ArgumentException("WarehouseId não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(zoneName))
            throw new ArgumentException("Nome da zona não pode ser vazio");

        Id = Guid.NewGuid();
        WarehouseId = warehouseId;
        ZoneName = zoneName;
        Type = type;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string ZoneName { get; private set; } = string.Empty;
    public ZoneType Type { get; private set; }
    public decimal? Temperature { get; private set; }
    public decimal? Humidity { get; private set; }
    public decimal TotalCapacity { get; private set; }
    public decimal UsedCapacity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Warehouse Warehouse { get; private set; } = null!;
    public ICollection<StorageLocation> StorageLocations { get; private set; } = new List<StorageLocation>();

    public void Update(string zoneName, ZoneType type, decimal? temperature, decimal? humidity, decimal totalCapacity)
    {
        if (string.IsNullOrWhiteSpace(zoneName))
            throw new ArgumentException("Nome da zona não pode ser vazio");

        ZoneName = zoneName;
        Type = type;
        Temperature = temperature;
        Humidity = humidity;
        TotalCapacity = totalCapacity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUsedCapacity(decimal usedCapacity)
    {
        if (usedCapacity < 0 || usedCapacity > TotalCapacity)
            throw new ArgumentException("Capacidade usada inválida");
        
        UsedCapacity = usedCapacity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
