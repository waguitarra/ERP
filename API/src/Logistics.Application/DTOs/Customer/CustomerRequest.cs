using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Customer;

public class CustomerRequest
{
    [Required] public Guid CompanyId { get; set; }
    [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string Document { get; set; } = string.Empty;
    [MaxLength(20)] public string? Phone { get; set; }
    [MaxLength(100)] public string? Email { get; set; }
    [MaxLength(500)] public string? Address { get; set; }
}
