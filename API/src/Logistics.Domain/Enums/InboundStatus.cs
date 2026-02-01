namespace Logistics.Domain.Enums;

public enum InboundStatus
{
    Scheduled = 1,
    InProgress = 2,
    InTransit = 2,      // Alias para InProgress
    Receiving = 2,      // Alias para InProgress (em recebimento)
    Completed = 3,
    Received = 3,       // Alias para Completed
    Cancelled = 4
}
