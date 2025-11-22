using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class PutawayTask
{
    private PutawayTask() { } // EF Core

    public PutawayTask(string taskNumber, Guid receiptId, Guid productId, decimal quantity, Guid fromLocationId, Guid toLocationId)
    {
        if (string.IsNullOrWhiteSpace(taskNumber)) throw new ArgumentException("TaskNumber inválido");
        if (receiptId == Guid.Empty) throw new ArgumentException("ReceiptId inválido");
        if (productId == Guid.Empty) throw new ArgumentException("ProductId inválido");
        if (quantity <= 0) throw new ArgumentException("Quantidade inválida");

        Id = Guid.NewGuid();
        TaskNumber = taskNumber;
        ReceiptId = receiptId;
        ProductId = productId;
        Quantity = quantity;
        FromLocationId = fromLocationId;
        ToLocationId = toLocationId;
        Priority = TaskPriority.Normal;
        Status = WMSTaskStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string TaskNumber { get; private set; } = string.Empty;
    public Guid ReceiptId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? LotId { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid FromLocationId { get; private set; } // Staging area
    public Guid ToLocationId { get; private set; } // Final location
    public TaskPriority Priority { get; private set; }
    public WMSTaskStatus Status { get; private set; }
    public Guid? AssignedTo { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Receipt Receipt { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    public Lot? Lot { get; private set; }
    public StorageLocation FromLocation { get; private set; } = null!;
    public StorageLocation ToLocation { get; private set; } = null!;

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

    public void SetLot(Guid lotId)
    {
        LotId = lotId;
        UpdatedAt = DateTime.UtcNow;
    }
}
