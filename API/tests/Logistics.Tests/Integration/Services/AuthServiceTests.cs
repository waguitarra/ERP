using FluentAssertions;
using Logistics.Application.DTOs.Auth;
using Logistics.Application.Services;
using Logistics.Domain.Enums;
using Logistics.Infrastructure.Repositories;
using Logistics.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Logistics.Tests.Integration.Services;

[Trait("Category", "Integration")]
[Trait("Layer", "Application")]
public class AuthServiceTests : IDisposable
{
    private readonly Infrastructure.Data.LogisticsDbContext _context;
    private readonly UnitOfWork _unitOfWork;
    private readonly AuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        _context = TestDbContextFactory.Create();
        _unitOfWork = new UnitOfWork(_context);

        // Configurar mock do IConfiguration
        var inMemorySettings = new Dictionary<string, string>
        {
            {"JwtSettings:Secret", "test-secret-key-with-at-least-32-characters-for-testing"},
            {"JwtSettings:Issuer", "TestIssuer"},
            {"JwtSettings:Audience", "TestAudience"},
            {"JwtSettings:ExpirationHours", "8"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _authService = new AuthService(_unitOfWork, _configuration);
    }

    [Fact]
    public async Task RegisterAdminAsync_WithValidData_ShouldCreateAdminUser()
    {
        // Arrange
        var request = new RegisterAdminRequest
        {
            Name = "Admin Master",
            Email = "admin@test.com",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123"
        };

        // Act
        var result = await _authService.RegisterAdminAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(request.Email);
        result.Name.Should().Be(request.Name);
        result.Role.Should().Be(UserRole.Admin.ToString());
        result.CompanyId.Should().BeNull();

        // Verificar se foi salvo no banco
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        user.Should().NotBeNull();
        user!.Role.Should().Be(UserRole.Admin);
    }

    [Fact]
    public async Task RegisterAdminAsync_WhenAdminAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var firstAdmin = new RegisterAdminRequest
        {
            Name = "First Admin",
            Email = "admin1@test.com",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123"
        };
        await _authService.RegisterAdminAsync(firstAdmin);

        var secondAdmin = new RegisterAdminRequest
        {
            Name = "Second Admin",
            Email = "admin2@test.com",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123"
        };

        // Act
        Func<Task> act = async () => await _authService.RegisterAdminAsync(secondAdmin);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Já existe um administrador master cadastrado*");
    }

    [Fact]
    public async Task RegisterAdminAsync_WithMismatchedPasswords_ShouldThrowException()
    {
        // Arrange
        var request = new RegisterAdminRequest
        {
            Name = "Admin Master",
            Email = "admin@test.com",
            Password = "Admin@123",
            ConfirmPassword = "DifferentPassword"
        };

        // Act
        Func<Task> act = async () => await _authService.RegisterAdminAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*As senhas não coincidem*");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var registerRequest = new RegisterAdminRequest
        {
            Name = "Admin Test",
            Email = "admin@test.com",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123"
        };
        await _authService.RegisterAdminAsync(registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "admin@test.com",
            Password = "Admin@123"
        };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(loginRequest.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@test.com",
            Password = "Admin@123"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginRequest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Credenciais inválidas*");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var registerRequest = new RegisterAdminRequest
        {
            Name = "Admin Test",
            Email = "admin@test.com",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123"
        };
        await _authService.RegisterAdminAsync(registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "admin@test.com",
            Password = "WrongPassword"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginRequest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Credenciais inválidas*");
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var registerRequest = new RegisterAdminRequest
        {
            Name = "Admin Test",
            Email = "admin@test.com",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123"
        };
        await _authService.RegisterAdminAsync(registerRequest);

        // Desativar usuário
        var user = await _unitOfWork.Users.GetByEmailAsync("admin@test.com");
        user!.Deactivate();
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CommitAsync();

        var loginRequest = new LoginRequest
        {
            Email = "admin@test.com",
            Password = "Admin@123"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(loginRequest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Credenciais inválidas*");
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@test.com";
        var role = "Admin";
        Guid? companyId = null;

        // Act
        var token = _authService.GenerateJwtToken(userId, email, role, companyId);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT tem 3 partes
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
