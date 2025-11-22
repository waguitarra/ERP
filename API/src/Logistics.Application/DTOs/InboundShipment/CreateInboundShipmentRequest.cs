namespace Logistics.Application.DTOs.InboundShipment;

public record CreateInboundShipmentRequest(
    Guid CompanyId,
    string ShipmentNumber,
    Guid OrderId,
    Guid SupplierId,
    Guid? VehicleId,
    Guid? DriverId,
    DateTime? ExpectedArrivalDate,
    string? DockDoorNumber,
    string? ASNNumber
);
