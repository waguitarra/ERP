namespace Logistics.Domain.Enums;

public enum PickingLineStatus
{
    Pending = 0,
    InProgress = 1,
    Partial = 2,
    Picked = 3,
    Completed = 3,  // Alias para Picked
    Cancelled = 4
}
