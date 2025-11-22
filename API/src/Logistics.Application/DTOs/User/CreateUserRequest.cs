using System.ComponentModel.DataAnnotations;
using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.User;

public record CreateUserRequest(
    [Required] Guid CompanyId,
    [Required, MaxLength(200)] string Name,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] UserRole Role
);
