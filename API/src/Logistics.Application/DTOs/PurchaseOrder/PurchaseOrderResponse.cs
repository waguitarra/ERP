using OrderPriorityEnum = Logistics.Domain.Enums.OrderPriority;
using OrderStatusEnum = Logistics.Domain.Enums.OrderStatus;
using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.PurchaseOrder;

public class PurchaseOrderResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public OrderPriorityEnum Priority { get; set; }
    public OrderStatusEnum Status { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public List<PurchaseOrderItemResponse> Items { get; set; } = new();
}

public record PurchaseOrderItemResponse(
    Guid Id,
    Guid ProductId,
    string SKU,
    decimal QuantityOrdered,
    decimal QuantityReceived,
    decimal UnitPrice
);