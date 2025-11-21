namespace Logistics.Domain.Entities;

public class Product
{
    private Product() { } // EF Core

    public Product(Guid companyId, string name, string sku, string? barcode = null)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU não pode ser vazio");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        Name = name;
        SKU = sku;
        Barcode = barcode;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string SKU { get; private set; } = string.Empty;
    public string? Barcode { get; private set; }
    public string? Description { get; private set; }
    public decimal Weight { get; private set; }
    public string? WeightUnit { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Company Company { get; private set; } = null!;

    public void Update(string name, string sku, string? barcode, string? description, decimal weight, string? weightUnit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU não pode ser vazio");

        Name = name;
        SKU = sku;
        Barcode = barcode;
        Description = description;
        Weight = weight;
        WeightUnit = weightUnit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
