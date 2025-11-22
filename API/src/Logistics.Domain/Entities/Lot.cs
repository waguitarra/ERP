using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class Lot
{
    private Lot() { } // EF Core

    public Lot(Guid companyId, string lotNumber, Guid productId, DateTime manufactureDate, DateTime expiryDate, decimal quantityReceived)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(lotNumber))
            throw new ArgumentException("Número do lote não pode ser vazio");
        
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId não pode ser vazio");
        
        if (quantityReceived <= 0)
            throw new ArgumentException("Quantidade recebida deve ser maior que zero");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        LotNumber = lotNumber;
        ProductId = productId;
        ManufactureDate = manufactureDate;
        ExpiryDate = expiryDate;
        QuantityReceived = quantityReceived;
        QuantityAvailable = quantityReceived;
        Status = LotStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string LotNumber { get; private set; } = string.Empty;
    public Guid ProductId { get; private set; }
    public DateTime ManufactureDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public decimal QuantityReceived { get; private set; }
    public decimal QuantityAvailable { get; private set; }
    public LotStatus Status { get; private set; }
    public Guid? SupplierId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    public Supplier? Supplier { get; private set; }

    public void SetSupplier(Guid supplierId)
    {
        SupplierId = supplierId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAvailableQuantity(decimal quantityAvailable)
    {
        if (quantityAvailable < 0 || quantityAvailable > QuantityReceived)
            throw new ArgumentException("Quantidade disponível inválida");
        
        QuantityAvailable = quantityAvailable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(LotStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void QuarantineLot()
    {
        Status = LotStatus.Quarantine;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseLot()
    {
        if (ExpiryDate < DateTime.UtcNow)
        {
            Status = LotStatus.Expired;
        }
        else
        {
            Status = LotStatus.Available;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsExpired()
    {
        Status = LotStatus.Expired;
        QuantityAvailable = 0;
        UpdatedAt = DateTime.UtcNow;
    }
}
