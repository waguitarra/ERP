using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class PickingTask
{
    private PickingTask() { } // EF Core

    public PickingTask(string taskNumber, Guid pickingWaveId, Guid orderId)
    {
        if (string.IsNullOrWhiteSpace(taskNumber)) throw new ArgumentException("TaskNumber inválido");
        if (pickingWaveId == Guid.Empty) throw new ArgumentException("PickingWaveId inválido");
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId inválido");

        Id = Guid.NewGuid();
        TaskNumber = taskNumber;
        PickingWaveId = pickingWaveId;
        OrderId = orderId;
        Priority = TaskPriority.Normal;
        Status = WMSTaskStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string TaskNumber { get; private set; } = string.Empty;
    public Guid PickingWaveId { get; private set; }
    public Guid OrderId { get; private set; }
    public TaskPriority Priority { get; private set; }
    public WMSTaskStatus Status { get; private set; }
    public Guid? AssignedTo { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public PickingWave PickingWave { get; private set; } = null!;
    public Order Order { get; private set; } = null!;
    public ICollection<PickingLine> Lines { get; private set; } = new List<PickingLine>();

    public void AssignTo(Guid userId)
    {
        AssignedTo = userId;
        Status = WMSTaskStatus.Assigned;
        UpdatedAt = DateTime.UtcNow;
    }

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

    public void Cancel()
    {
        Status = WMSTaskStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
