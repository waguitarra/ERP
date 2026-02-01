namespace Logistics.Domain.Enums;

public enum OrderSource
{
    Manual = 1,       // Criado manualmente
    ERP = 2,          // Importado do ERP
    Ecommerce = 3,    // E-commerce
    Web = 3,          // Alias para Ecommerce (compatibilidade BD)
    API = 4,          // Via API externa
    EDI = 5,          // EDI
    WMS = 6,          // WMS (Warehouse Management System)
    Phone = 7         // Pedido por telefone
}
