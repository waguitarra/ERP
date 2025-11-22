using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class ReceiptLine
{
    private ReceiptLine() { } // EF Core

    public ReceiptLine(Guid receiptId, Guid productId, string sku, decimal quantityExpected)
    {
        if (receiptId == Guid.Empty) throw new ArgumentException("ReceiptId inválido");
        if (productId == Guid.Empty) throw new ArgumentException("ProductId inválido");
        if (quantityExpected <= 0) throw new ArgumentException("Quantidade esperada inválida");

        Id = Guid.NewGuid();
        ReceiptId = receiptId;
        ProductId = productId;
        SKU = sku;
        QuantityExpected = quantityExpected;
        QuantityReceived = 0;
        QuantityDamaged = 0;
        InspectionStatus = InspectionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid ReceiptId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public string? LotNumber { get; private set; }
    public string? SerialNumber { get; private set; }
    public decimal QuantityExpected { get; private set; }
    public decimal QuantityReceived { get; private set; }
    public decimal QuantityDamaged { get; private set; }
    public InspectionStatus InspectionStatus { get; private set; }
    public string? QualityNotes { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Receipt Receipt { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public void SetLotAndSerial(string? lotNumber, string? serialNumber, DateTime? expiryDate)
    {
        LotNumber = lotNumber;
        SerialNumber = serialNumber;
        ExpiryDate = expiryDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReceiveQuantity(decimal received, decimal damaged)
    {
        QuantityReceived = received;
        QuantityDamaged = damaged;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetInspectionResult(InspectionStatus status, string? notes)
    {
        InspectionStatus = status;
        QualityNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
