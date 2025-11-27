namespace Logistics.Domain.Entities;

public class SalesOrderItem
{
    private SalesOrderItem() { } // EF Core

    public SalesOrderItem(Guid productId, string sku, decimal quantityOrdered, decimal unitPrice)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId inválido");
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU inválido");
        if (quantityOrdered <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");
        if (unitPrice < 0)
            throw new ArgumentException("Preço unitário não pode ser negativo");

        Id = Guid.NewGuid();
        ProductId = productId;
        SKU = sku;
        QuantityOrdered = quantityOrdered;
        UnitPrice = unitPrice;
        QuantityAllocated = 0;
        QuantityPicked = 0;
        QuantityShipped = 0;
    }

    public Guid Id { get; private set; }
    public Guid SalesOrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public decimal QuantityOrdered { get; private set; }
    public decimal QuantityAllocated { get; private set; }
    public decimal QuantityPicked { get; private set; }
    public decimal QuantityShipped { get; private set; }
    public decimal UnitPrice { get; private set; }

    // Navigation
    public SalesOrder SalesOrder { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public void UpdateQuantityAllocated(decimal quantityAllocated)
    {
        if (quantityAllocated < 0)
            throw new ArgumentException("Quantidade alocada não pode ser negativa");
        if (quantityAllocated > QuantityOrdered)
            throw new ArgumentException("Quantidade alocada não pode ser maior que a ordenada");

        QuantityAllocated = quantityAllocated;
    }

    public void UpdateQuantityPicked(decimal quantityPicked)
    {
        if (quantityPicked < 0)
            throw new ArgumentException("Quantidade separada não pode ser negativa");
        if (quantityPicked > QuantityOrdered)
            throw new ArgumentException("Quantidade separada não pode ser maior que a ordenada");

        QuantityPicked = quantityPicked;
    }

    public void UpdateQuantityShipped(decimal quantityShipped)
    {
        if (quantityShipped < 0)
            throw new ArgumentException("Quantidade enviada não pode ser negativa");
        if (quantityShipped > QuantityOrdered)
            throw new ArgumentException("Quantidade enviada não pode ser maior que a ordenada");

        QuantityShipped = quantityShipped;
    }
}
