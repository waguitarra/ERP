using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.Driver;

public class DriverRequest
{
    [Required(ErrorMessage = "CompanyId é obrigatório")]
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Número da CNH é obrigatório")]
    [StringLength(20, ErrorMessage = "CNH deve ter no máximo 20 caracteres")]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    [Phone(ErrorMessage = "Formato de telefone inválido")]
    public string Phone { get; set; } = string.Empty;
}
