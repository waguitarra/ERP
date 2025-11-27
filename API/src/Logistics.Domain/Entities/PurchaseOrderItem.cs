namespace Logistics.Domain.Entities;

public class PurchaseOrderItem
{
    private PurchaseOrderItem() { } // EF Core

    public PurchaseOrderItem(Guid productId, string sku, decimal quantityOrdered, decimal unitPrice)
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
        QuantityReceived = 0;
    }

    public Guid Id { get; private set; }
    public Guid PurchaseOrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public decimal QuantityOrdered { get; private set; }
    public decimal QuantityReceived { get; private set; }
    public decimal UnitPrice { get; private set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public void UpdateQuantityReceived(decimal quantityReceived)
    {
        if (quantityReceived < 0)
            throw new ArgumentException("Quantidade recebida não pode ser negativa");
        if (quantityReceived > QuantityOrdered)
            throw new ArgumentException("Quantidade recebida não pode ser maior que a ordenada");

        QuantityReceived = quantityReceived;
    }
}
