using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class StorageLocation
{
    private StorageLocation() { }

    public StorageLocation(Guid warehouseId, Guid? zoneId, string code, string? description = null)
    {
        if (warehouseId == Guid.Empty)
            throw new ArgumentException("WarehouseId não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código não pode ser vazio");

        Id = Guid.NewGuid();
        WarehouseId = warehouseId;
        ZoneId = zoneId;
        Code = code;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? ZoneId { get; private set; }
    public string Code { get; private set; } = string.Empty; // Ex: A-01-2-B
    public string? Description { get; private set; }
    
    // WMS Structure
    public string? Aisle { get; private set; }      // Corredor: A, B, C
    public string? Rack { get; private set; }       // Rack: 01, 02, 03
    public string? Level { get; private set; }      // Nível: 1, 2, 3
    public string? Position { get; private set; }   // Posição: A, B, C
    public LocationType Type { get; private set; }
    
    // Capacity
    public decimal MaxWeight { get; private set; }
    public decimal MaxVolume { get; private set; }
    public decimal CurrentWeight { get; private set; }
    public decimal CurrentVolume { get; private set; }
    public bool IsBlocked { get; private set; }
    public string? BlockReason { get; private set; }
    
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Warehouse Warehouse { get; private set; } = null!;
    public WarehouseZone? Zone { get; private set; }

    public void Update(string code, string? description)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código não pode ser vazio");

        Code = code;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetStructure(string? aisle, string? rack, string? level, string? position)
    {
        Aisle = aisle;
        Rack = rack;
        Level = level;
        Position = position;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetCapacity(decimal maxWeight, decimal maxVolume, LocationType type)
    {
        if (maxWeight < 0) throw new ArgumentException("Peso máximo inválido");
        if (maxVolume < 0) throw new ArgumentException("Volume máximo inválido");
        
        MaxWeight = maxWeight;
        MaxVolume = maxVolume;
        Type = type;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateCurrentUsage(decimal currentWeight, decimal currentVolume)
    {
        if (currentWeight < 0 || currentWeight > MaxWeight)
            throw new ArgumentException("Peso atual inválido");
        if (currentVolume < 0 || currentVolume > MaxVolume)
            throw new ArgumentException("Volume atual inválido");
        
        CurrentWeight = currentWeight;
        CurrentVolume = currentVolume;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Block(string reason)
    {
        IsBlocked = true;
        BlockReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Unblock()
    {
        IsBlocked = false;
        BlockReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
