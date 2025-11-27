namespace Logistics.Domain.Entities;

public class InboundParcelItem
{
    private InboundParcelItem() { } // EF Core

    public InboundParcelItem(Guid inboundParcelId, Guid productId, string sku, decimal quantity)
    {
        if (inboundParcelId == Guid.Empty)
            throw new ArgumentException("InboundParcelId inválido");
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId inválido");
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU inválido");
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");

        Id = Guid.NewGuid();
        InboundParcelId = inboundParcelId;
        ProductId = productId;
        SKU = sku;
        ExpectedQuantity = quantity;
        ReceivedQuantity = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid InboundParcelId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public decimal ExpectedQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public InboundParcel InboundParcel { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public void IncrementReceived(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");
        
        ReceivedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
