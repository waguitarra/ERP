namespace Logistics.Application.DTOs.Receipt;

public record CreateReceiptRequest(
    string ReceiptNumber,
    Guid InboundShipmentId,
    Guid WarehouseId,
    Guid ReceivedBy
);
