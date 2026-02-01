namespace Logistics.Domain.Enums;

public enum OrderType
{
    Inbound = 1,      // Entrada (recebimento)
    Purchase = 1,     // Alias para compatibilidade com dados existentes
    Outbound = 2,     // Saída (expedição)
    Sales = 2,        // Alias para compatibilidade com dados existentes
    Transfer = 3,     // Transferência entre armazéns
    Return = 4        // Devolução
}
