namespace Logistics.Application.DTOs.InboundShipment;

public record InboundShipmentResponse(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    string ShipmentNumber,
    Guid OrderId,
    string OrderNumber,
    Guid SupplierId,
    string SupplierName,
    Guid? VehicleId,
    string? VehiclePlate,
    Guid? DriverId,
    string? DriverName,
    DateTime? ExpectedArrivalDate,
    DateTime? ActualArrivalDate,
    string? DockDoorNumber,
    int Status,
    string StatusName,
    decimal TotalQuantityExpected,
    decimal TotalQuantityReceived,
    string? ASNNumber,
    bool HasQualityIssues,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
