using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class InboundParcel
{
    private InboundParcel() { } // EF Core

    public InboundParcel(Guid inboundShipmentId, string parcelNumber, ParcelType type)
    {
        if (inboundShipmentId == Guid.Empty)
            throw new ArgumentException("InboundShipmentId inválido");
        if (string.IsNullOrWhiteSpace(parcelNumber))
            throw new ArgumentException("ParcelNumber inválido");

        Id = Guid.NewGuid();
        InboundShipmentId = inboundShipmentId;
        ParcelNumber = parcelNumber;
        Type = type;
        Status = ParcelStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid InboundShipmentId { get; private set; }
    public string ParcelNumber { get; private set; } = string.Empty;
    public ParcelType Type { get; private set; }
    public string? LPN { get; private set; } // License Plate Number (SSCC)
    
    public int SequenceNumber { get; private set; }
    public int TotalParcels { get; private set; }
    
    public Guid? ParentParcelId { get; private set; }
    
    public decimal Weight { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public string? DimensionUnit { get; private set; }
    
    public ParcelStatus Status { get; private set; }
    public string? CurrentLocation { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    
    public bool HasDamage { get; private set; }
    public string? DamageNotes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public InboundShipment InboundShipment { get; private set; } = null!;
    public InboundParcel? ParentParcel { get; private set; }
    public ICollection<InboundParcel> ChildParcels { get; private set; } = new List<InboundParcel>();
    public ICollection<InboundParcelItem> Items { get; private set; } = new List<InboundParcelItem>();
    public ICollection<InboundCarton> Cartons { get; private set; } = new List<InboundCarton>();

    public void SetLPN(string lpn)
    {
        if (string.IsNullOrWhiteSpace(lpn))
            throw new ArgumentException("LPN inválido");
        LPN = lpn;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSequence(int sequenceNumber, int totalParcels)
    {
        if (sequenceNumber <= 0 || totalParcels <= 0)
            throw new ArgumentException("Sequência inválida");
        if (sequenceNumber > totalParcels)
            throw new ArgumentException("Sequência não pode ser maior que total");

        SequenceNumber = sequenceNumber;
        TotalParcels = totalParcels;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDimensions(decimal weight, decimal length, decimal width, decimal height, string unit = "cm")
    {
        if (weight < 0 || length < 0 || width < 0 || height < 0)
            throw new ArgumentException("Dimensões não podem ser negativas");
        
        Weight = weight;
        Length = length;
        Width = width;
        Height = height;
        DimensionUnit = unit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsReceived(Guid receivedBy, string? location = null)
    {
        Status = ParcelStatus.Received;
        ReceivedAt = DateTime.UtcNow;
        ReceivedBy = receivedBy;
        CurrentLocation = location;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReportDamage(string notes)
    {
        HasDamage = true;
        DamageNotes = notes;
        Status = ParcelStatus.Damaged;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(ParcelStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetLocation(string location)
    {
        CurrentLocation = location;
        UpdatedAt = DateTime.UtcNow;
    }
}
