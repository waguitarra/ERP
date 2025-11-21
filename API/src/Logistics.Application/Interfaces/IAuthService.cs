using Logistics.Application.DTOs.Auth;

namespace Logistics.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAdminAsync(RegisterAdminRequest request);
    string GenerateJwtToken(Guid userId, string email, string role, Guid? companyId);
}
