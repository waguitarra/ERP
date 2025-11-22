using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.PackingTask;

public record PackingTaskResponse(
    Guid Id,
    string TaskNumber,
    Guid OrderId,
    string OrderNumber,
    WMSTaskStatus Status,
    Guid AssignedTo,
    DateTime? CompletedAt,
    DateTime CreatedAt
);
