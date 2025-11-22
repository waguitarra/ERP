namespace Logistics.Domain.Enums;

public enum OrderStatus
{
    Draft = 1,              // Rascunho
    Pending = 2,            // Pendente
    Confirmed = 3,          // Confirmado
    InProgress = 4,         // Em progresso
    PartiallyFulfilled = 5, // Parcialmente atendido
    Fulfilled = 6,          // Atendido
    Shipped = 7,            // Enviado
    Delivered = 8,          // Entregue
    Cancelled = 9,          // Cancelado
    OnHold = 10            // Em espera
}
