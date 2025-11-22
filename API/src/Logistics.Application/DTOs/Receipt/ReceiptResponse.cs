using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.Receipt;

public record ReceiptResponse(
    Guid Id,
    string ReceiptNumber,
    Guid InboundShipmentId,
    string ShipmentNumber,
    DateTime ReceiptDate,
    ReceiptStatus Status,
    Guid WarehouseId,
    string WarehouseName,
    Guid ReceivedBy,
    List<ReceiptLineResponse> Lines,
    DateTime CreatedAt
);

public record ReceiptLineResponse(
    Guid Id,
    Guid ProductId,
    string SKU,
    string ProductName,
    string? LotNumber,
    decimal QuantityExpected,
    decimal QuantityReceived,
    decimal QuantityDamaged,
    InspectionStatus InspectionStatus,
    DateTime? ExpiryDate
);
