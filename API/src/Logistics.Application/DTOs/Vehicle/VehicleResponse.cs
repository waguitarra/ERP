namespace Logistics.Application.DTOs.Vehicle;

public class VehicleResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Relacionamento opcional
    public string? CompanyName { get; set; }
}
