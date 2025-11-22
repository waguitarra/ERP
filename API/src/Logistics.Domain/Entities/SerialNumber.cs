using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class SerialNumber
{
    private SerialNumber() { } // EF Core

    public SerialNumber(string serial, Guid productId, Guid lotId)
    {
        if (string.IsNullOrWhiteSpace(serial)) throw new ArgumentException("Serial inválido");
        if (productId == Guid.Empty) throw new ArgumentException("ProductId inválido");

        Id = Guid.NewGuid();
        Serial = serial;
        ProductId = productId;
        LotId = lotId;
        Status = SerialStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Serial { get; private set; } = string.Empty;
    public Guid ProductId { get; private set; }
    public Guid LotId { get; private set; }
    public SerialStatus Status { get; private set; }
    public Guid? CurrentLocationId { get; private set; }
    public DateTime? ReceivedDate { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Product Product { get; private set; } = null!;
    public Lot Lot { get; private set; } = null!;
    public StorageLocation? CurrentLocation { get; private set; }

    public void Receive(DateTime receivedDate, Guid locationId)
    {
        ReceivedDate = receivedDate;
        CurrentLocationId = locationId;
        Status = SerialStatus.InStock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ship(DateTime shippedDate)
    {
        ShippedDate = shippedDate;
        Status = SerialStatus.Sold;
        UpdatedAt = DateTime.UtcNow;
    }
}
