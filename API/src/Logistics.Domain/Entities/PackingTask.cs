using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class PackingTask
{
    private PackingTask() { } // EF Core

    public PackingTask(string taskNumber, Guid orderId, Guid assignedTo)
    {
        if (string.IsNullOrWhiteSpace(taskNumber)) throw new ArgumentException("TaskNumber inválido");
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId inválido");
        if (assignedTo == Guid.Empty) throw new ArgumentException("AssignedTo inválido");

        Id = Guid.NewGuid();
        TaskNumber = taskNumber;
        OrderId = orderId;
        AssignedTo = assignedTo;
        Status = WMSTaskStatus.Assigned;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string TaskNumber { get; private set; } = string.Empty;
    public Guid OrderId { get; private set; }
    public WMSTaskStatus Status { get; private set; }
    public Guid AssignedTo { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Order Order { get; private set; } = null!;
    public ICollection<Package> Packages { get; private set; } = new List<Package>();

    public void Start()
    {
        Status = WMSTaskStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = WMSTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
