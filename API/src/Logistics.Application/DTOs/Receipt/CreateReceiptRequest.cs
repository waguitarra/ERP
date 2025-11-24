namespace Logistics.Application.DTOs.Receipt;

public class CreateReceiptRequest
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public Guid InboundShipmentId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ReceivedBy { get; set; }
}
