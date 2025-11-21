using FluentAssertions;
using Logistics.Domain.Entities;
using Xunit;

namespace Logistics.Tests.Unit.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class CompanyTests
{
    [Fact]
    public void CreateCompany_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "Transportadora XYZ";
        var document = "12345678901234";

        // Act
        var company = new Company(name, document);

        // Assert
        company.Should().NotBeNull();
        company.Id.Should().NotBeEmpty();
        company.Name.Should().Be(name);
        company.Document.Should().Be(document);
        company.IsActive.Should().BeTrue();
        company.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateCompany_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var name = "";
        var document = "12345678901234";

        // Act
        Action act = () => new Company(name, document);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Nome da empresa é obrigatório*");
    }

    [Fact]
    public void CreateCompany_WithEmptyDocument_ShouldThrowException()
    {
        // Arrange
        var name = "Transportadora XYZ";
        var document = "";

        // Act
        Action act = () => new Company(name, document);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Documento da empresa é obrigatório*");
    }

    [Fact]
    public void CreateCompany_WithInvalidDocument_ShouldThrowException()
    {
        // Arrange
        var name = "Transportadora XYZ";
        var document = "123"; // Menos de 14 dígitos

        // Act
        Action act = () => new Company(name, document);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Documento deve ser um CNPJ válido*");
    }

    [Fact]
    public void UpdateCompany_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var company = new Company("Nome Antigo", "12345678901234");
        var newName = "Nome Novo";
        var newDocument = "98765432109876";

        // Act
        company.Update(newName, newDocument);

        // Assert
        company.Name.Should().Be(newName);
        company.Document.Should().Be(newDocument);
        company.UpdatedAt.Should().NotBeNull();
        company.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void DeactivateCompany_ShouldSetIsActiveFalse()
    {
        // Arrange
        var company = new Company("Transportadora XYZ", "12345678901234");

        // Act
        company.Deactivate();

        // Assert
        company.IsActive.Should().BeFalse();
    }

    [Fact]
    public void ActivateCompany_ShouldSetIsActiveTrue()
    {
        // Arrange
        var company = new Company("Transportadora XYZ", "12345678901234");
        company.Deactivate();

        // Act
        company.Activate();

        // Assert
        company.IsActive.Should().BeTrue();
    }
}
