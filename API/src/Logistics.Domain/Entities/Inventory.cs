namespace Logistics.Domain.Entities;

public class Inventory
{
    private Inventory() { }

    public Inventory(Guid productId, Guid storageLocationId, decimal quantity)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId não pode ser vazio");
        
        if (storageLocationId == Guid.Empty)
            throw new ArgumentException("StorageLocationId não pode ser vazio");
        
        if (quantity < 0)
            throw new ArgumentException("Quantidade não pode ser negativa");

        Id = Guid.NewGuid();
        ProductId = productId;
        StorageLocationId = storageLocationId;
        Quantity = quantity;
        ReservedQuantity = 0;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid StorageLocationId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    public Product Product { get; private set; } = null!;
    public StorageLocation StorageLocation { get; private set; } = null!;

    public void AddStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser positiva");

        Quantity += quantity;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser positiva");

        var availableQuantity = Quantity - ReservedQuantity;
        if (quantity > availableQuantity)
            throw new InvalidOperationException($"Quantidade insuficiente em estoque. Disponível: {availableQuantity}");

        Quantity -= quantity;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void Reserve(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser positiva");

        var availableQuantity = Quantity - ReservedQuantity;
        if (quantity > availableQuantity)
            throw new InvalidOperationException($"Quantidade insuficiente para reserva. Disponível: {availableQuantity}");

        ReservedQuantity += quantity;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseReservation(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser positiva");

        if (quantity > ReservedQuantity)
            throw new InvalidOperationException("Quantidade de liberação maior que a reservada");

        ReservedQuantity -= quantity;
        LastUpdatedAt = DateTime.UtcNow;
    }
}
