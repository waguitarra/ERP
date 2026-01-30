using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Vehicle;

public class VehicleRequest
{
    [Required(ErrorMessage = "CompanyId é obrigatório")]
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Placa é obrigatória")]
    [StringLength(10, ErrorMessage = "Placa deve ter no máximo 10 caracteres")]
    public string LicensePlate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Modelo é obrigatório")]
    [StringLength(100, ErrorMessage = "Modelo deve ter no máximo 100 caracteres")]
    public string Model { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Marca deve ter no máximo 50 caracteres")]
    public string? Brand { get; set; }

    [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
    public string? VehicleType { get; set; }

    [Required(ErrorMessage = "Ano é obrigatório")]
    [Range(1900, 2100, ErrorMessage = "Ano deve estar entre 1900 e 2100")]
    public int Year { get; set; }

    public decimal? Capacity { get; set; }

    [StringLength(30, ErrorMessage = "Cor deve ter no máximo 30 caracteres")]
    public string? Color { get; set; }

    [StringLength(30, ErrorMessage = "Tipo de combustível deve ter no máximo 30 caracteres")]
    public string? FuelType { get; set; }

    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Notes { get; set; }
    
    public bool TrackingEnabled { get; set; } = false;
}

public class UpdateVehicleLocationRequest
{
    [Required]
    public double Latitude { get; set; }
    
    [Required]
    public double Longitude { get; set; }
    
    public double? Speed { get; set; }
    public string? Address { get; set; }
}

public class AssignDriverRequest
{
    [Required(ErrorMessage = "Nome do motorista é obrigatório")]
    [StringLength(100)]
    public string DriverName { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string? DriverPhone { get; set; }
}
