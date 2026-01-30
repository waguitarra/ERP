using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class InboundShipment
{
    private InboundShipment() { } // EF Core

    public InboundShipment(Guid companyId, string shipmentNumber, Guid orderId, Guid supplierId)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido");
        if (string.IsNullOrWhiteSpace(shipmentNumber)) throw new ArgumentException("ShipmentNumber inválido");
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId inválido");
        if (supplierId == Guid.Empty) throw new ArgumentException("SupplierId inválido");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        ShipmentNumber = shipmentNumber;
        OrderId = orderId;
        SupplierId = supplierId;
        Status = InboundStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string ShipmentNumber { get; private set; } = string.Empty;
    public Guid OrderId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid? VehicleId { get; private set; }
    public Guid? DriverId { get; private set; }
    public DateTime? ExpectedArrivalDate { get; private set; }
    public DateTime? ActualArrivalDate { get; private set; }
    public string? DockDoorNumber { get; private set; }
    public InboundStatus Status { get; private set; }
    public decimal TotalQuantityExpected { get; private set; }
    public decimal TotalQuantityReceived { get; private set; }
    public string? ASNNumber { get; private set; } // Advanced Shipping Notice
    public bool HasQualityIssues { get; private set; }
    public Guid? InspectedBy { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;
    public Order Order { get; private set; } = null!;
    public Supplier Supplier { get; private set; } = null!;
    public Vehicle? Vehicle { get; private set; }
    public Driver? Driver { get; private set; }

    public void SetVehicleAndDriver(Guid? vehicleId, Guid? driverId)
    {
        VehicleId = vehicleId;
        DriverId = driverId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetExpectedArrival(DateTime expectedDate, string? dockDoorNumber)
    {
        ExpectedArrivalDate = expectedDate;
        DockDoorNumber = dockDoorNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActualArrival(DateTime actualDate, Guid receivedBy)
    {
        ActualArrivalDate = actualDate;
        ReceivedBy = receivedBy;
        Status = InboundStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetASN(string asnNumber)
    {
        ASNNumber = asnNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetQualityIssue(bool hasIssues, Guid? inspectedBy)
    {
        HasQualityIssues = hasIssues;
        InspectedBy = inspectedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantities(decimal expected, decimal received)
    {
        TotalQuantityExpected = expected;
        TotalQuantityReceived = received;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = InboundStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = InboundStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
