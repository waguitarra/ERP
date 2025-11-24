namespace Logistics.Application.DTOs.InboundShipment;

public class CreateInboundShipmentRequest
{
    public Guid CompanyId { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime? ExpectedArrivalDate { get; set; }
    public string? DockDoorNumber { get; set; }
    public string? ASNNumber { get; set; }
}
