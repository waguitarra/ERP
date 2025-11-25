using OrderPriorityEnum = Logistics.Domain.Enums.OrderPriority;
using OrderStatusEnum = Logistics.Domain.Enums.OrderStatus;

namespace Logistics.Application.DTOs.Order;

public record UpdateOrderRequest(
    OrderStatusEnum? Status,
    OrderPriorityEnum? Priority,
    Guid? VehicleId,
    Guid? DriverId,
    Guid? OriginWarehouseId,
    Guid? DestinationWarehouseId,
    string? ShippingZipCode,
    decimal? ShippingLatitude,
    decimal? ShippingLongitude,
    string? ShippingCity,
    string? ShippingState,
    string? ShippingCountry,
    string? TrackingNumber,
    DateTime? EstimatedDeliveryDate,
    DateTime? ActualDeliveryDate,
    string? ShippingAddress,
    string? SpecialInstructions
);
