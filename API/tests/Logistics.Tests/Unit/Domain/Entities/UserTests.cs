using FluentAssertions;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Xunit;

namespace Logistics.Tests.Unit.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class UserTests
{
    [Fact]
    public void CreateUser_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "João Silva";
        var email = "joao@example.com";
        var passwordHash = "hashedpassword";
        var role = UserRole.CompanyUser;
        var companyId = Guid.NewGuid();

        // Act
        var user = new User(name, email, passwordHash, role, companyId);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeEmpty();
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(role);
        user.CompanyId.Should().Be(companyId);
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateAdminUser_WithoutCompanyId_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "Admin Master";
        var email = "admin@example.com";
        var passwordHash = "hashedpassword";
        var role = UserRole.Admin;

        // Act
        var user = new User(name, email, passwordHash, role, null);

        // Assert
        user.Should().NotBeNull();
        user.Role.Should().Be(UserRole.Admin);
        user.CompanyId.Should().BeNull();
    }

    [Fact]
    public void CreateAdminUser_WithCompanyId_ShouldThrowException()
    {
        // Arrange
        var name = "Admin Master";
        var email = "admin@example.com";
        var passwordHash = "hashedpassword";
        var role = UserRole.Admin;
        var companyId = Guid.NewGuid();

        // Act
        Action act = () => new User(name, email, passwordHash, role, companyId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Admin Master não pode estar vinculado a uma empresa*");
    }

    [Fact]
    public void CreateCompanyUser_WithoutCompanyId_ShouldThrowException()
    {
        // Arrange
        var name = "João Silva";
        var email = "joao@example.com";
        var passwordHash = "hashedpassword";
        var role = UserRole.CompanyUser;

        // Act
        Action act = () => new User(name, email, passwordHash, role, null);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Usuários de empresa devem estar vinculados a uma empresa*");
    }

    [Fact]
    public void CreateUser_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var name = "João Silva";
        var email = "invalid-email";
        var passwordHash = "hashedpassword";
        var role = UserRole.CompanyUser;
        var companyId = Guid.NewGuid();

        // Act
        Action act = () => new User(name, email, passwordHash, role, companyId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email inválido*");
    }

    [Fact]
    public void UpdatePassword_ShouldUpdatePasswordHash()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "oldHash", UserRole.CompanyUser, Guid.NewGuid());
        var newPasswordHash = "newHash";

        // Act
        user.UpdatePassword(newPasswordHash);

        // Assert
        user.PasswordHash.Should().Be(newPasswordHash);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateLastLogin_ShouldSetLastLoginAt()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "hash", UserRole.CompanyUser, Guid.NewGuid());

        // Act
        user.UpdateLastLogin();

        // Assert
        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void DeactivateUser_ShouldSetIsActiveFalse()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "hash", UserRole.CompanyUser, Guid.NewGuid());

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
    }
}
