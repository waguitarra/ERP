namespace Logistics.Application.DTOs.PackingTask;

public class CreatePackingTaskRequest
{
    public string TaskNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid AssignedTo { get; set; }
}
