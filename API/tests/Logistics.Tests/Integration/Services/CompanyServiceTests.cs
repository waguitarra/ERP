using FluentAssertions;
using Logistics.Application.DTOs.Company;
using Logistics.Application.Services;
using Logistics.Infrastructure.Repositories;
using Logistics.Tests.Helpers;
using Xunit;

namespace Logistics.Tests.Integration.Services;

[Trait("Category", "Integration")]
[Trait("Layer", "Application")]
public class CompanyServiceTests : IDisposable
{
    private readonly Infrastructure.Data.LogisticsDbContext _context;
    private readonly UnitOfWork _unitOfWork;
    private readonly CompanyService _companyService;

    public CompanyServiceTests()
    {
        _context = TestDbContextFactory.Create();
        _unitOfWork = new UnitOfWork(_context);
        _companyService = new CompanyService(_unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateCompany()
    {
        // Arrange
        var request = new CompanyRequest
        {
            Name = "Test Company",
            Document = "12345678901234"
        };

        // Act
        var result = await _companyService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Document.Should().Be(request.Document);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateDocument_ShouldThrowException()
    {
        // Arrange
        var firstRequest = new CompanyRequest
        {
            Name = "First Company",
            Document = "12345678901234"
        };
        await _companyService.CreateAsync(firstRequest);

        var duplicateRequest = new CompanyRequest
        {
            Name = "Second Company",
            Document = "12345678901234" // Mesmo documento
        };

        // Act
        Func<Task> act = async () => await _companyService.CreateAsync(duplicateRequest);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Documento já cadastrado*");
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnCompany()
    {
        // Arrange
        var created = await _companyService.CreateAsync(new CompanyRequest
        {
            Name = "Test Company",
            Document = "12345678901234"
        });

        // Act
        var result = await _companyService.GetByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCompanies()
    {
        // Arrange
        await _companyService.CreateAsync(new CompanyRequest { Name = "Company 1", Document = "11111111111111" });
        await _companyService.CreateAsync(new CompanyRequest { Name = "Company 2", Document = "22222222222222" });

        // Act
        var result = await _companyService.GetAllAsync();

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
