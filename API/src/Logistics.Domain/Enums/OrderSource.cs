namespace Logistics.Domain.Enums;

public enum OrderSource
{
    Manual = 1,       // Criado manualmente
    ERP = 2,          // Importado do ERP
    Ecommerce = 3,    // E-commerce
    API = 4,          // Via API externa
    EDI = 5           // EDI
}
