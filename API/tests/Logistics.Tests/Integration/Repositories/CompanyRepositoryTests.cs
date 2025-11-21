using FluentAssertions;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Repositories;
using Logistics.Tests.Helpers;
using Xunit;

namespace Logistics.Tests.Integration.Repositories;

[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class CompanyRepositoryTests : IDisposable
{
    private readonly Infrastructure.Data.LogisticsDbContext _context;
    private readonly CompanyRepository _repository;

    public CompanyRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new CompanyRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidCompany_ShouldAddToDatabase()
    {
        // Arrange
        var company = FakeDataGenerator.GenerateCompany();

        // Act
        await _repository.AddAsync(company);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(company.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be(company.Name);
        result.Document.Should().Be(company.Document);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCompanyExists_ShouldReturnCompany()
    {
        // Arrange
        var company = FakeDataGenerator.GenerateCompany();
        await _repository.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(company.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCompanyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByDocumentAsync_WhenDocumentExists_ShouldReturnCompany()
    {
        // Arrange
        var company = FakeDataGenerator.GenerateCompany();
        await _repository.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDocumentAsync(company.Document);

        // Assert
        result.Should().NotBeNull();
        result!.Document.Should().Be(company.Document);
    }

    [Fact]
    public async Task DocumentExistsAsync_WhenDocumentExists_ShouldReturnTrue()
    {
        // Arrange
        var company = FakeDataGenerator.GenerateCompany();
        await _repository.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.DocumentExistsAsync(company.Document);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task DocumentExistsAsync_WhenDocumentDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentDocument = "99999999999999";

        // Act
        var exists = await _repository.DocumentExistsAsync(nonExistentDocument);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveCompanies()
    {
        // Arrange
        var activeCompany1 = FakeDataGenerator.GenerateCompany(isActive: true);
        var activeCompany2 = FakeDataGenerator.GenerateCompany(isActive: true);
        var inactiveCompany = FakeDataGenerator.GenerateCompany(isActive: false);

        await _repository.AddAsync(activeCompany1);
        await _repository.AddAsync(activeCompany2);
        await _repository.AddAsync(inactiveCompany);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Id == activeCompany1.Id);
        result.Should().Contain(c => c.Id == activeCompany2.Id);
        result.Should().NotContain(c => c.Id == inactiveCompany.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCompanyInDatabase()
    {
        // Arrange
        var company = FakeDataGenerator.GenerateCompany();
        await _repository.AddAsync(company);
        await _context.SaveChangesAsync();

        var newName = "Novo Nome da Empresa";
        company.Update(newName, company.Document);

        // Act
        await _repository.UpdateAsync(company);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(company.Id);
        updated!.Name.Should().Be(newName);
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCompanyFromDatabase()
    {
        // Arrange
        var company = FakeDataGenerator.GenerateCompany();
        await _repository.AddAsync(company);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(company.Id);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(company.Id);
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
