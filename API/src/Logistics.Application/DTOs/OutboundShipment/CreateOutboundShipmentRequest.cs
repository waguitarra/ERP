namespace Logistics.Application.DTOs.OutboundShipment;

public class CreateOutboundShipmentRequest
{
    public string ShipmentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid CarrierId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? DeliveryAddress { get; set; }
}
