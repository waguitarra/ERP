using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.StockMovement;

public class StockMovementRequest
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? StorageLocationId { get; set; }
    public StockMovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}
