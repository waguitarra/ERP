using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.PutawayTask;

public record PutawayTaskResponse(
    Guid Id,
    string TaskNumber,
    Guid ReceiptId,
    Guid ProductId,
    string ProductName,
    decimal Quantity,
    Guid FromLocationId,
    Guid ToLocationId,
    WMSTaskStatus Status,
    Guid? AssignedTo,
    DateTime? CompletedAt,
    DateTime CreatedAt
);
