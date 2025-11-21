using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Logistics.Application.DTOs.Auth;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace Logistics.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Credenciais inválidas");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciais inválidas");

        user.UpdateLastLogin();
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CommitAsync();

        var token = GenerateJwtToken(user.Id, user.Email, user.Role.ToString(), user.CompanyId);

        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role.ToString(),
            CompanyId = user.CompanyId
        };
    }

    public async Task<LoginResponse> RegisterAdminAsync(RegisterAdminRequest request)
    {
        // Verificar se já existe admin
        var existingAdmin = (await _unitOfWork.Users.FindAsync(u => u.Role == UserRole.Admin)).FirstOrDefault();
        if (existingAdmin != null)
            throw new InvalidOperationException("Já existe um administrador master cadastrado");

        if (request.Password != request.ConfirmPassword)
            throw new ArgumentException("As senhas não coincidem");

        if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            throw new InvalidOperationException("Email já cadastrado");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var admin = new User(
            request.Name,
            request.Email,
            passwordHash,
            UserRole.Admin,
            null
        );

        await _unitOfWork.Users.AddAsync(admin);
        await _unitOfWork.CommitAsync();

        var token = GenerateJwtToken(admin.Id, admin.Email, admin.Role.ToString(), null);

        return new LoginResponse
        {
            Token = token,
            Email = admin.Email,
            Name = admin.Name,
            Role = admin.Role.ToString(),
            CompanyId = null
        };
    }

    public string GenerateJwtToken(Guid userId, string email, string role, Guid? companyId)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado");
        var issuer = jwtSettings["Issuer"] ?? "LogisticsAPI";
        var audience = jwtSettings["Audience"] ?? "LogisticsClient";
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "8");

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (companyId.HasValue)
        {
            claims.Add(new Claim("CompanyId", companyId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
