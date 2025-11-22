using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class Receipt
{
    private Receipt() { } // EF Core

    public Receipt(string receiptNumber, Guid inboundShipmentId, Guid warehouseId, Guid receivedBy)
    {
        if (string.IsNullOrWhiteSpace(receiptNumber)) throw new ArgumentException("ReceiptNumber inválido");
        if (inboundShipmentId == Guid.Empty) throw new ArgumentException("InboundShipmentId inválido");
        if (warehouseId == Guid.Empty) throw new ArgumentException("WarehouseId inválido");
        if (receivedBy == Guid.Empty) throw new ArgumentException("ReceivedBy inválido");

        Id = Guid.NewGuid();
        ReceiptNumber = receiptNumber;
        InboundShipmentId = inboundShipmentId;
        WarehouseId = warehouseId;
        ReceivedBy = receivedBy;
        ReceiptDate = DateTime.UtcNow;
        Status = ReceiptStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string ReceiptNumber { get; private set; } = string.Empty; // GRN - Goods Receipt Note
    public Guid InboundShipmentId { get; private set; }
    public DateTime ReceiptDate { get; private set; }
    public ReceiptStatus Status { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid ReceivedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public InboundShipment InboundShipment { get; private set; } = null!;
    public Warehouse Warehouse { get; private set; } = null!;
    public ICollection<ReceiptLine> Lines { get; private set; } = new List<ReceiptLine>();

    public void SetStatus(ReceiptStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLine(ReceiptLine line)
    {
        Lines.Add(line);
        UpdatedAt = DateTime.UtcNow;
    }
}
