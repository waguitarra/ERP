using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Product;

public class ProductRequest
{
    [Required] public Guid CompanyId { get; set; }
    [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(50)] public string SKU { get; set; } = string.Empty;
    [MaxLength(50)] public string? Barcode { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    public decimal Weight { get; set; }
    [MaxLength(10)] public string? WeightUnit { get; set; }
}
