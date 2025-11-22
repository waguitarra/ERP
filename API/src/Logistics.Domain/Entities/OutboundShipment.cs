using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class OutboundShipment
{
    private OutboundShipment() { } // EF Core

    public OutboundShipment(string shipmentNumber, Guid orderId, Guid carrierId)
    {
        if (string.IsNullOrWhiteSpace(shipmentNumber)) throw new ArgumentException("ShipmentNumber inválido");
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId inválido");
        if (carrierId == Guid.Empty) throw new ArgumentException("CarrierId inválido");

        Id = Guid.NewGuid();
        ShipmentNumber = shipmentNumber;
        OrderId = orderId;
        CarrierId = carrierId;
        Status = OutboundStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string ShipmentNumber { get; private set; } = string.Empty;
    public Guid OrderId { get; private set; }
    public Guid CarrierId { get; private set; }
    public string? TrackingNumber { get; private set; }
    public OutboundStatus Status { get; private set; }
    public DateTime? ShippedDate { get; private set; }
    public DateTime? DeliveredDate { get; private set; }
    public string? DeliveryAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Order Order { get; private set; } = null!;

    public void SetTracking(string trackingNumber)
    {
        TrackingNumber = trackingNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ship(DateTime shippedDate)
    {
        ShippedDate = shippedDate;
        Status = OutboundStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deliver(DateTime deliveredDate)
    {
        DeliveredDate = deliveredDate;
        Status = OutboundStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAddress(string address)
    {
        DeliveryAddress = address;
        UpdatedAt = DateTime.UtcNow;
    }
}
