namespace Logistics.Application.DTOs.Inventory;

public class InventoryRequest
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? StorageLocationId { get; set; }
    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
}
