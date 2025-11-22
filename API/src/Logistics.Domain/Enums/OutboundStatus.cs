namespace Logistics.Domain.Enums;

public enum OutboundStatus
{
    Pending = 1,
    ReadyToShip = 2,
    Shipped = 3,
    InTransit = 4,
    Delivered = 5,
    Cancelled = 6
}
