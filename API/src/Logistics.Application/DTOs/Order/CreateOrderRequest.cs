using OrderPriorityEnum = Logistics.Domain.Enums.OrderPriority;
using OrderStatusEnum = Logistics.Domain.Enums.OrderStatus;
using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Order;

public record CreateOrderRequest(
    Guid CompanyId,
    string OrderNumber,
    OrderType Type,
    OrderSource Source,
    Guid? CustomerId,
    Guid? SupplierId,
    DateTime? ExpectedDate,
    OrderPriorityEnum Priority,
    string? ShippingAddress,
    string? SpecialInstructions,
    bool IsBOPIS,
    List<CreateOrderItemRequest> Items
);

public record CreateOrderItemRequest(
    Guid ProductId,
    string SKU,
    decimal QuantityOrdered,
    decimal UnitPrice,
    string? RequiredLotNumber,
    DateTime? RequiredShipDate
);
