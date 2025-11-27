using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class InboundCarton
{
    private InboundCarton() { } // EF Core

    public InboundCarton(Guid inboundParcelId, string cartonNumber, int sequenceNumber, int totalCartons)
    {
        if (inboundParcelId == Guid.Empty)
            throw new ArgumentException("InboundParcelId inválido");
        if (string.IsNullOrWhiteSpace(cartonNumber))
            throw new ArgumentException("CartonNumber inválido");
        if (sequenceNumber <= 0 || totalCartons <= 0)
            throw new ArgumentException("Sequência inválida");
        if (sequenceNumber > totalCartons)
            throw new ArgumentException("Sequência não pode ser maior que total");

        Id = Guid.NewGuid();
        InboundParcelId = inboundParcelId;
        CartonNumber = cartonNumber;
        SequenceNumber = sequenceNumber;
        TotalCartons = totalCartons;
        Status = CartonStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid InboundParcelId { get; private set; }
    public string CartonNumber { get; private set; } = string.Empty;
    public string? Barcode { get; private set; }
    public int SequenceNumber { get; private set; }
    public int TotalCartons { get; private set; }
    
    public decimal Weight { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    
    public CartonStatus Status { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    
    public bool HasDamage { get; private set; }
    public string? DamageNotes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public InboundParcel InboundParcel { get; private set; } = null!;
    public ICollection<InboundCartonItem> Items { get; private set; } = new List<InboundCartonItem>();

    public void SetBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("Barcode inválido");
        Barcode = barcode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDimensions(decimal weight, decimal length, decimal width, decimal height)
    {
        if (weight < 0 || length < 0 || width < 0 || height < 0)
            throw new ArgumentException("Dimensões não podem ser negativas");
        
        Weight = weight;
        Length = length;
        Width = width;
        Height = height;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsReceived(Guid receivedBy)
    {
        Status = CartonStatus.Received;
        ReceivedAt = DateTime.UtcNow;
        ReceivedBy = receivedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReportDamage(string notes)
    {
        HasDamage = true;
        DamageNotes = notes;
        Status = CartonStatus.Damaged;
        UpdatedAt = DateTime.UtcNow;
    }
}
