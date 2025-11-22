namespace Logistics.Application.DTOs.OutboundShipment;

public record CreateOutboundShipmentRequest(
    string ShipmentNumber,
    Guid OrderId,
    Guid CarrierId,
    string? TrackingNumber,
    string? DeliveryAddress
);
