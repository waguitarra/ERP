namespace Logistics.Application.DTOs.Product;

public class ProductResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public decimal Weight { get; set; }
    public string? WeightUnit { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
