namespace Logistics.Domain.Entities;

public class OrderItem
{
    private OrderItem() { } // EF Core

    public OrderItem(Guid orderId, Guid productId, string sku, decimal quantityOrdered, decimal unitPrice)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId não pode ser vazio");
        
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU não pode ser vazio");
        
        if (quantityOrdered <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        SKU = sku;
        QuantityOrdered = quantityOrdered;
        UnitPrice = unitPrice;
        QuantityAllocated = 0;
        QuantityPicked = 0;
        QuantityShipped = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public decimal QuantityOrdered { get; private set; }
    public decimal QuantityAllocated { get; private set; }
    public decimal QuantityPicked { get; private set; }
    public decimal QuantityShipped { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string? RequiredLotNumber { get; private set; }
    public DateTime? RequiredShipDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public void UpdateQuantity(decimal quantityOrdered)
    {
        if (quantityOrdered <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");
        
        QuantityOrdered = quantityOrdered;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AllocateQuantity(decimal quantity)
    {
        if (quantity < 0 || quantity > QuantityOrdered)
            throw new ArgumentException("Quantidade para alocar inválida");
        
        QuantityAllocated = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PickQuantity(decimal quantity)
    {
        if (quantity < 0 || quantity > QuantityAllocated)
            throw new ArgumentException("Quantidade para separar inválida");
        
        QuantityPicked = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ShipQuantity(decimal quantity)
    {
        if (quantity < 0 || quantity > QuantityPicked)
            throw new ArgumentException("Quantidade para enviar inválida");
        
        QuantityShipped = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRequirements(string? lotNumber, DateTime? shipDate)
    {
        RequiredLotNumber = lotNumber;
        RequiredShipDate = shipDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
