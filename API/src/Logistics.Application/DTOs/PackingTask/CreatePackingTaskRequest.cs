namespace Logistics.Application.DTOs.PackingTask;

public record CreatePackingTaskRequest(
    string TaskNumber,
    Guid OrderId,
    Guid AssignedTo
);
