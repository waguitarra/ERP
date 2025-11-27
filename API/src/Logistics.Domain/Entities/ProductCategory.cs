namespace Logistics.Domain.Entities;

public class ProductCategory
{
    private ProductCategory() { } // EF Core

    public ProductCategory(string name, string code, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome da categoria não pode ser vazio");
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código da categoria não pode ser vazio");

        Id = Guid.NewGuid();
        Name = name;
        Code = code;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Barcode { get; private set; }
    public string? Reference { get; private set; }
    public bool IsMaintenance { get; private set; }
    public bool IsActive { get; private set; }
    public string? Attributes { get; private set; } // JSON com atributos extras
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    public void Update(string name, string code, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio");
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código não pode ser vazio");

        Name = name;
        Code = code;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBarcode(string barcode)
    {
        Barcode = barcode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetReference(string reference)
    {
        Reference = reference;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMaintenance(bool isMaintenance)
    {
        IsMaintenance = isMaintenance;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAttributes(string? attributes)
    {
        Attributes = attributes;
        UpdatedAt = DateTime.UtcNow;
    }
}
