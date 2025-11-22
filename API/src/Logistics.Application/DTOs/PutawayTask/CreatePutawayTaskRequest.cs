namespace Logistics.Application.DTOs.PutawayTask;

public record CreatePutawayTaskRequest(
    string TaskNumber,
    Guid ReceiptId,
    Guid ProductId,
    decimal Quantity,
    Guid FromLocationId,
    Guid ToLocationId,
    Guid? LotId
);
