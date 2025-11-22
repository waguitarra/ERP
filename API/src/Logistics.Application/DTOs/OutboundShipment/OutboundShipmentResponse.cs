using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.OutboundShipment;

public record OutboundShipmentResponse(
    Guid Id,
    string ShipmentNumber,
    Guid OrderId,
    string OrderNumber,
    Guid CarrierId,
    string? TrackingNumber,
    OutboundStatus Status,
    DateTime? ShippedDate,
    DateTime? DeliveredDate,
    string? DeliveryAddress,
    DateTime CreatedAt
);
