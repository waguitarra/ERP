using FluentAssertions;
using Logistics.Application.DTOs.Driver;
using Logistics.Application.Services;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Data;
using Logistics.Infrastructure.Repositories;
using Logistics.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Logistics.Tests.Integration.Services;

[Collection("Database")]
public class DriverServiceTests : IDisposable
{
    private readonly LogisticsDbContext _context;
    private readonly DriverService _service;
    private readonly Company _testCompany;

    public DriverServiceTests()
    {
        var options = new DbContextOptionsBuilder<LogisticsDbContext>()
            .UseMySql(
                "Server=localhost;Database=logistics_db;User=logistics_user;Password=password;",
                ServerVersion.AutoDetect("Server=localhost;Database=logistics_db;User=logistics_user;Password=password;")
            )
            .Options;

        _context = new LogisticsDbContext(options);
        
        var driverRepository = new DriverRepository(_context);
        var companyRepository = new CompanyRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        
        _service = new DriverService(driverRepository, companyRepository, unitOfWork);

        // Criar empresa de teste
        _testCompany = FakeDataGenerator.GenerateCompany();
        _context.Companies.Add(_testCompany);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateDriver()
    {
        // Arrange
        var request = new DriverRequest
        {
            CompanyId = _testCompany.Id,
            Name = "Carlos Silva",
            LicenseNumber = "12345678901",
            Phone = "+5511999999999"
        };

        // Act
        var response = await _service.CreateAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
        response.CompanyId.Should().Be(_testCompany.Id);
        response.Name.Should().Be("Carlos Silva");
        response.LicenseNumber.Should().Be("12345678901");
        response.Phone.Should().Be("+5511999999999");
        response.IsActive.Should().BeTrue();
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verificar no banco
        var driverInDb = await _context.Drivers.FindAsync(response.Id);
        driverInDb.Should().NotBeNull();
        driverInDb!.Name.Should().Be("Carlos Silva");
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentCompany_ShouldThrowException()
    {
        // Arrange
        var request = new DriverRequest
        {
            CompanyId = Guid.NewGuid(),
            Name = "João Silva",
            LicenseNumber = "99999999999",
            Phone = "+5511988888888"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateLicenseNumber_ShouldThrowException()
    {
        // Arrange
        var driver1 = new Driver(_testCompany.Id, "Maria Santos", "11111111111", "+5511977777777");
        _context.Drivers.Add(driver1);
        await _context.SaveChangesAsync();

        var request = new DriverRequest
        {
            CompanyId = _testCompany.Id,
            Name = "Outro Motorista",
            LicenseNumber = "11111111111", // Duplicada
            Phone = "+5511966666666"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _service.CreateAsync(request));
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnDriver()
    {
        // Arrange
        var driver = new Driver(_testCompany.Id, "Pedro Lima", "22222222222", "+5511955555555");
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.GetByIdAsync(driver.Id);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(driver.Id);
        response.Name.Should().Be("Pedro Lima");
        response.LicenseNumber.Should().Be("22222222222");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDrivers()
    {
        // Arrange
        var driver1 = new Driver(_testCompany.Id, "Driver 1", "33333333333", "+5511944444444");
        var driver2 = new Driver(_testCompany.Id, "Driver 2", "44444444444", "+5511933333333");
        _context.Drivers.AddRange(driver1, driver2);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.GetAllAsync();

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveCountGreaterOrEqualTo(2);
        response.Should().Contain(d => d.LicenseNumber == "33333333333");
        response.Should().Contain(d => d.LicenseNumber == "44444444444");
    }

    [Fact]
    public async Task GetByCompanyIdAsync_ShouldReturnOnlyCompanyDrivers()
    {
        // Arrange - Criar outra empresa
        var otherCompany = FakeDataGenerator.GenerateCompany();
        _context.Companies.Add(otherCompany);
        await _context.SaveChangesAsync();
        
        var uniqueId = Guid.NewGuid().ToString().Substring(0, 11);
        var driver1 = new Driver(_testCompany.Id, "Driver 1", $"55{uniqueId}", "+5511922222222");
        var driver2 = new Driver(_testCompany.Id, "Driver 2", $"66{uniqueId}", "+5511911111111");
        var driver3 = new Driver(otherCompany.Id, "Other Driver", $"77{uniqueId}", "+5511900000000");
        
        _context.Drivers.AddRange(driver1, driver2, driver3);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.GetByCompanyIdAsync(_testCompany.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveCountGreaterOrEqualTo(2);
        response.Should().Contain(d => d.LicenseNumber.Contains(uniqueId));
        response.Count().Should().BeGreaterOrEqualTo(2);
        
        // Limpar drivers da outra empresa
        _context.Drivers.Remove(driver3);
        _context.Companies.Remove(otherCompany);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateDriver()
    {
        // Arrange
        var driver = new Driver(_testCompany.Id, "Old Name", "88888888888", "+5511999998888");
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        var request = new DriverRequest
        {
            CompanyId = _testCompany.Id,
            Name = "New Name",
            LicenseNumber = "99999999999",
            Phone = "+5511999997777"
        };

        // Act
        var response = await _service.UpdateAsync(driver.Id, request);

        // Assert
        response.Should().NotBeNull();
        response.Name.Should().Be("New Name");
        response.LicenseNumber.Should().Be("99999999999");
        response.Phone.Should().Be("+5511999997777");
        response.UpdatedAt.Should().NotBeNull();

        // Verificar no banco
        var driverInDb = await _context.Drivers.FindAsync(driver.Id);
        driverInDb!.Name.Should().Be("New Name");
        driverInDb.LicenseNumber.Should().Be("99999999999");
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ShouldThrowException()
    {
        // Arrange
        var request = new DriverRequest
        {
            CompanyId = _testCompany.Id,
            Name = "Test",
            LicenseNumber = "00000000000",
            Phone = "+5511999996666"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _service.UpdateAsync(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task ActivateAsync_ShouldSetIsActiveTrue()
    {
        // Arrange
        var driver = new Driver(_testCompany.Id, "Test Driver", "10101010101", "+5511999995555");
        driver.Deactivate();
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        await _service.ActivateAsync(driver.Id);

        // Assert
        var driverInDb = await _context.Drivers.FindAsync(driver.Id);
        driverInDb!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateAsync_ShouldSetIsActiveFalse()
    {
        // Arrange
        var driver = new Driver(_testCompany.Id, "Test Driver", "20202020202", "+5511999994444");
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        await _service.DeactivateAsync(driver.Id);

        // Assert
        var driverInDb = await _context.Drivers.FindAsync(driver.Id);
        driverInDb!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveDriver()
    {
        // Arrange
        var driver = new Driver(_testCompany.Id, "Delete Driver", "30303030303", "+5511999993333");
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        await _service.DeleteAsync(driver.Id);

        // Assert
        var driverInDb = await _context.Drivers.FindAsync(driver.Id);
        driverInDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _service.DeleteAsync(Guid.NewGuid()));
    }

    public void Dispose()
    {
        try
        {
            // Limpar dados de teste na ordem correta (children primeiro)
            var drivers = _context.Drivers.Where(d => d.CompanyId == _testCompany.Id).ToList();
            foreach (var driver in drivers)
            {
                _context.Drivers.Remove(driver);
            }
            _context.SaveChanges();
            
            var company = _context.Companies.Find(_testCompany.Id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                _context.SaveChanges();
            }
        }
        catch
        {
            // Ignorar erros de limpeza
        }
        finally
        {
            _context.Dispose();
        }
    }
}
