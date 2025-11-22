namespace Logistics.Application.DTOs.Inventory;

public class InventoryResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public Guid? StorageLocationId { get; set; }
    public string? StorageLocationCode { get; set; }
    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public DateTime LastUpdated { get; set; }
}
