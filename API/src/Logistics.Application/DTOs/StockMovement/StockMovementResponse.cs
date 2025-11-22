using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.StockMovement;

public class StockMovementResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public Guid? StorageLocationId { get; set; }
    public string? StorageLocationCode { get; set; }
    public StockMovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public DateTime MovementDate { get; set; }
}
