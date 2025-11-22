using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.InboundShipment;

public record InboundShipmentResponse(
    Guid Id,
    Guid CompanyId,
    string ShipmentNumber,
    Guid OrderId,
    Guid SupplierId,
    string SupplierName,
    Guid? VehicleId,
    Guid? DriverId,
    DateTime? ExpectedArrivalDate,
    DateTime? ActualArrivalDate,
    string? DockDoorNumber,
    InboundStatus Status,
    decimal TotalQuantityExpected,
    decimal TotalQuantityReceived,
    string? ASNNumber,
    bool HasQualityIssues,
    DateTime CreatedAt
);
