namespace Logistics.Domain.Enums;

public enum OrderType
{
    Inbound = 1,      // Entrada (recebimento)
    Outbound = 2,     // Saída (expedição)
    Transfer = 3,     // Transferência entre armazéns
    Return = 4        // Devolução
}
