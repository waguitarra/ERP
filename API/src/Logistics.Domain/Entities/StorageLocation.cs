namespace Logistics.Domain.Entities;

public class StorageLocation
{
    private StorageLocation() { }

    public StorageLocation(Guid warehouseId, string code, string? description = null)
    {
        if (warehouseId == Guid.Empty)
            throw new ArgumentException("WarehouseId não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código não pode ser vazio");

        Id = Guid.NewGuid();
        WarehouseId = warehouseId;
        Code = code;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Warehouse Warehouse { get; private set; } = null!;

    public void Update(string code, string? description)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código não pode ser vazio");

        Code = code;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
