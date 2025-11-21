using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class StockMovement
{
    private StockMovement() { }

    public StockMovement(
        Guid productId, 
        Guid storageLocationId, 
        StockMovementType type, 
        decimal quantity,
        string? reference = null,
        string? notes = null)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId não pode ser vazio");
        
        if (storageLocationId == Guid.Empty)
            throw new ArgumentException("StorageLocationId não pode ser vazio");
        
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser positiva");

        Id = Guid.NewGuid();
        ProductId = productId;
        StorageLocationId = storageLocationId;
        Type = type;
        Quantity = quantity;
        Reference = reference;
        Notes = notes;
        MovementDate = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid StorageLocationId { get; private set; }
    public StockMovementType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public string? Reference { get; private set; }
    public string? Notes { get; private set; }
    public DateTime MovementDate { get; private set; }

    public Product Product { get; private set; } = null!;
    public StorageLocation StorageLocation { get; private set; } = null!;
}
