namespace Logistics.Domain.Enums;

public enum CartonStatus
{
    Pending = 0,       // Aguardando recebimento
    Receiving = 1,     // Sendo recebido (em processo)
    Received = 2,      // Recebido completamente
    Damaged = 3,       // Danificado
    Quarantine = 4,    // Em quarentena
    Stored = 5         // Armazenado
}
