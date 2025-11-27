namespace Logistics.Domain.Enums;

public enum ParcelStatus
{
    Pending = 0,       // Aguardando recebimento
    InTransit = 1,     // Em trânsito
    Receiving = 2,     // Sendo recebido (em processo)
    Received = 3,      // Recebido completamente
    Damaged = 4,       // Danificado
    Quarantine = 5,    // Em quarentena
    Stored = 6,        // Armazenado
    Lost = 7           // Perdido
}
