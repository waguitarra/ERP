namespace Logistics.Application.DTOs.PackingTask;

public record PackingTaskResponse(
    Guid Id,
    string TaskNumber,
    Guid OrderId,
    string OrderNumber,
    int Status,
    string StatusName,
    Guid AssignedTo,
    string AssignedToName,
    int PackageCount,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
