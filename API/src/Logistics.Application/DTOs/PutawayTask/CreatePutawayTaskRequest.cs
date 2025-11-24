namespace Logistics.Application.DTOs.PutawayTask;

public class CreatePutawayTaskRequest
{
    public string TaskNumber { get; set; } = string.Empty;
    public Guid ReceiptId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public Guid FromLocationId { get; set; }
    public Guid ToLocationId { get; set; }
    public Guid? LotId { get; set; }
}
