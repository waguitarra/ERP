using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class PickingLine
{
    private PickingLine() { } // EF Core

    public PickingLine(Guid pickingTaskId, Guid orderItemId, Guid productId, Guid locationId, decimal quantityToPick)
    {
        if (pickingTaskId == Guid.Empty) throw new ArgumentException("PickingTaskId inválido");
        if (productId == Guid.Empty) throw new ArgumentException("ProductId inválido");
        if (quantityToPick <= 0) throw new ArgumentException("Quantidade inválida");

        Id = Guid.NewGuid();
        PickingTaskId = pickingTaskId;
        OrderItemId = orderItemId;
        ProductId = productId;
        LocationId = locationId;
        QuantityToPick = quantityToPick;
        QuantityPicked = 0;
        Status = PickingLineStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PickingTaskId { get; private set; }
    public Guid OrderItemId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid LocationId { get; private set; }
    public Guid? LotId { get; private set; }
    public string? SerialNumber { get; private set; }
    public decimal QuantityToPick { get; private set; }
    public decimal QuantityPicked { get; private set; }
    public PickingLineStatus Status { get; private set; }
    public Guid? PickedBy { get; private set; }
    public DateTime? PickedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public PickingTask PickingTask { get; private set; } = null!;
    public OrderItem OrderItem { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    public StorageLocation Location { get; private set; } = null!;
    public Lot? Lot { get; private set; }

    public void Pick(decimal quantity, Guid? pickedBy, Guid? lotId = null, string? serialNumber = null)
    {
        QuantityPicked = quantity;
        PickedBy = pickedBy;
        PickedAt = DateTime.UtcNow;
        LotId = lotId;
        SerialNumber = serialNumber;
        Status = quantity >= QuantityToPick ? PickingLineStatus.Picked : PickingLineStatus.Partial;
        UpdatedAt = DateTime.UtcNow;
    }
}
