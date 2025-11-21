using System.ComponentModel.DataAnnotations;
namespace Logistics.Application.DTOs.Warehouse;
public class WarehouseRequest
{
    [Required] public Guid CompanyId { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string Code { get; set; } = string.Empty;
    [MaxLength(500)] public string? Address { get; set; }
}
