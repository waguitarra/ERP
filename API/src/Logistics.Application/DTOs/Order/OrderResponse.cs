using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Order;

public record OrderResponse(
    Guid Id,
    Guid CompanyId,
    string OrderNumber,
    OrderType Type,
    OrderSource Source,
    Guid? CustomerId,
    Guid? SupplierId,
    DateTime OrderDate,
    DateTime? ExpectedDate,
    OrderPriority Priority,
    OrderStatus Status,
    decimal TotalQuantity,
    decimal TotalValue,
    string? ShippingAddress,
    bool IsBOPIS,
    List<OrderItemResponse> Items,
    DateTime CreatedAt
);

public record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string SKU,
    decimal QuantityOrdered,
    decimal QuantityAllocated,
    decimal QuantityPicked,
    decimal QuantityShipped,
    decimal UnitPrice
);
