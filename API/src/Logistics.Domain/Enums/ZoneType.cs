namespace Logistics.Domain.Enums;

public enum ZoneType
{
    Receiving = 1,      // Área de recebimento
    Storage = 2,        // Estocagem geral
    Picking = 3,        // Área de separação
    Packing = 4,        // Área de embalagem
    Shipping = 5,       // Expedição
    Staging = 6,        // Staging/Buffer
    Returns = 7,        // Devoluções
    Quarantine = 8,     // Quarentena
    Refrigerated = 9,   // Refrigerado
    Hazmat = 10        // Materiais perigosos
}
