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
    
    // WMS Fields
    public decimal Volume { get; private set; }
    public string? VolumeUnit { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public string? DimensionUnit { get; private set; }
    public bool RequiresLotTracking { get; private set; }
    public bool RequiresSerialTracking { get; private set; }
    public bool IsPerishable { get; private set; }
    public int? ShelfLifeDays { get; private set; }
    public decimal? MinimumStock { get; private set; }
    public decimal? SafetyStock { get; private set; }
    public string? ABCClassification { get; private set; } // A, B, C
    
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
    
    public void UpdateWMSProperties(
        decimal volume, string? volumeUnit,
        decimal length, decimal width, decimal height, string? dimensionUnit,
        bool requiresLotTracking, bool requiresSerialTracking,
        bool isPerishable, int? shelfLifeDays,
        decimal? minimumStock, decimal? safetyStock,
        string? abcClassification)
    {
        Volume = volume;
        VolumeUnit = volumeUnit;
        Length = length;
        Width = width;
        Height = height;
        DimensionUnit = dimensionUnit;
        RequiresLotTracking = requiresLotTracking;
        RequiresSerialTracking = requiresSerialTracking;
        IsPerishable = isPerishable;
        ShelfLifeDays = shelfLifeDays;
        MinimumStock = minimumStock;
        SafetyStock = safetyStock;
        ABCClassification = abcClassification;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
