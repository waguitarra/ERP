namespace Logistics.Domain.Enums;

public enum OutboundStatus
{
    Draft = 0,
    Pending = 1,
    ReadyToShip = 2,
    Shipped = 3,
    InTransit = 4,
    Delivered = 5,
    Cancelled = 6
}
