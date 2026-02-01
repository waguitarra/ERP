namespace Logistics.Domain.Enums;

public enum OrderStatus
{
    Draft = 1,              // Rascunho
    Pending = 2,            // Pendente
    Confirmed = 3,          // Confirmado
    Approved = 4,           // Aprovado
    Processing = 5,         // Processando (alias InProgress)
    InProgress = 5,         // Em progresso
    PartiallyFulfilled = 6, // Parcialmente atendido
    Fulfilled = 7,          // Atendido
    Completed = 7,          // Completo (alias Fulfilled)
    InTransit = 8,          // Em trânsito (alias Shipped)
    Shipped = 8,            // Enviado
    Delivered = 9,          // Entregue
    Cancelled = 10,         // Cancelado
    OnHold = 11             // Em espera
}
