using OrderPriorityEnum = Logistics.Domain.Enums.OrderPriority;
using OrderStatusEnum = Logistics.Domain.Enums.OrderStatus;
using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Order;

public record OrderResponse(
    Guid Id,
    Guid CompanyId,
    string OrderNumber,
    OrderType Type,
    OrderSource Source,
    Guid? CustomerId,
    string? CustomerName,
    Guid? SupplierId,
    string? SupplierName,
    DateTime OrderDate,
    DateTime? ExpectedDate,
    OrderPriorityEnum Priority,
    OrderStatusEnum Status,
    decimal TotalQuantity,
    decimal TotalValue,
    string? ShippingAddress,
    string? SpecialInstructions,
    bool IsBOPIS,
    List<OrderItemResponse> Items,
    DateTime CreatedAt,
    // WMS Fields
    Guid? VehicleId,
    Guid? DriverId,
    Guid? OriginWarehouseId,
    Guid? DestinationWarehouseId,
    string? ShippingZipCode,
    string? ShippingCity,
    string? ShippingState,
    string? ShippingCountry,
    string? TrackingNumber,
    DateTime? EstimatedDeliveryDate
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
