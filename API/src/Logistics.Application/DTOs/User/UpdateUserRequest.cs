using System.ComponentModel.DataAnnotations;

namespace Logistics.Application.DTOs.User;

public record UpdateUserRequest(
    [Required, MaxLength(200)] string Name,
    [Required, EmailAddress] string Email
);
