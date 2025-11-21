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

    [Required(ErrorMessage = "Ano é obrigatório")]
    [Range(1900, 2100, ErrorMessage = "Ano deve estar entre 1900 e 2100")]
    public int Year { get; set; }
}
