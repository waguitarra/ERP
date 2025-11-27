namespace Logistics.Domain.Entities;

public class InboundCartonItem
{
    private InboundCartonItem() { } // EF Core

    public InboundCartonItem(Guid inboundCartonId, Guid productId, string sku, string? serialNumber = null)
    {
        if (inboundCartonId == Guid.Empty)
            throw new ArgumentException("InboundCartonId inválido");
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId inválido");
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU inválido");

        Id = Guid.NewGuid();
        InboundCartonId = inboundCartonId;
        ProductId = productId;
        SKU = sku;
        SerialNumber = serialNumber;
        IsReceived = false;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid InboundCartonId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public string? SerialNumber { get; private set; }
    public bool IsReceived { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public InboundCarton InboundCarton { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public void MarkAsReceived(Guid receivedBy)
    {
        IsReceived = true;
        ReceivedAt = DateTime.UtcNow;
        ReceivedBy = receivedBy;
    }

    public void SetSerialNumber(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new ArgumentException("Serial number inválido");
        SerialNumber = serialNumber;
    }
}
