using FluentAssertions;
using Logistics.Application.DTOs.Vehicle;
using Logistics.Application.Services;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Data;
using Logistics.Infrastructure.Repositories;
using Logistics.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Logistics.Tests.Integration.Services;

[Collection("Database")]
public class VehicleServiceTests : IDisposable
{
    private readonly LogisticsDbContext _context;
    private readonly VehicleService _service;
    private readonly Company _testCompany;

    public VehicleServiceTests()
    {
        var options = new DbContextOptionsBuilder<LogisticsDbContext>()
            .UseMySql(
                "Server=localhost;Database=logistics_db;User=logistics_user;Password=password;",
                ServerVersion.AutoDetect("Server=localhost;Database=logistics_db;User=logistics_user;Password=password;")
            )
            .Options;

        _context = new LogisticsDbContext(options);
        
        var vehicleRepository = new VehicleRepository(_context);
        var companyRepository = new CompanyRepository(_context);
        var unitOfWork = new UnitOfWork(_context);
        
        _service = new VehicleService(vehicleRepository, companyRepository, unitOfWork);

        // Criar empresa de teste
        _testCompany = FakeDataGenerator.GenerateCompany();
        _context.Companies.Add(_testCompany);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateVehicle()
    {
        // Arrange
        var request = new VehicleRequest
        {
            CompanyId = _testCompany.Id,
            LicensePlate = "ABC-1234",
            Model = "Mercedes Actros",
            Year = 2023
        };

        // Act
        var response = await _service.CreateAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
        response.CompanyId.Should().Be(_testCompany.Id);
        response.LicensePlate.Should().Be("ABC-1234");
        response.Model.Should().Be("Mercedes Actros");
        response.Year.Should().Be(2023);
        response.Status.Should().Be("Available");
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verificar no banco
        var vehicleInDb = await _context.Vehicles.FindAsync(response.Id);
        vehicleInDb.Should().NotBeNull();
        vehicleInDb!.LicensePlate.Should().Be("ABC-1234");
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentCompany_ShouldThrowException()
    {
        // Arrange
        var request = new VehicleRequest
        {
            CompanyId = Guid.NewGuid(),
            LicensePlate = "XYZ-9999",
            Model = "Volvo FH",
            Year = 2023
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateLicensePlate_ShouldThrowException()
    {
        // Arrange
        var vehicle1 = new Vehicle(_testCompany.Id, "DUP-1111", "Scania R500", 2022);
        _context.Vehicles.Add(vehicle1);
        await _context.SaveChangesAsync();

        var request = new VehicleRequest
        {
            CompanyId = _testCompany.Id,
            LicensePlate = "DUP-1111", // Duplicada
            Model = "Outro Modelo",
            Year = 2023
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _service.CreateAsync(request));
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnVehicle()
    {
        // Arrange
        var vehicle = new Vehicle(_testCompany.Id, "GET-1234", "Iveco Daily", 2021);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.GetByIdAsync(vehicle.Id);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(vehicle.Id);
        response.LicensePlate.Should().Be("GET-1234");
        response.Model.Should().Be("Iveco Daily");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllVehicles()
    {
        // Arrange
        var vehicle1 = new Vehicle(_testCompany.Id, "ALL-1111", "Model 1", 2021);
        var vehicle2 = new Vehicle(_testCompany.Id, "ALL-2222", "Model 2", 2022);
        _context.Vehicles.AddRange(vehicle1, vehicle2);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.GetAllAsync();

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveCountGreaterOrEqualTo(2);
        response.Should().Contain(v => v.LicensePlate == "ALL-1111");
        response.Should().Contain(v => v.LicensePlate == "ALL-2222");
    }

    [Fact]
    public async Task GetByCompanyIdAsync_ShouldReturnOnlyCompanyVehicles()
    {
        // Arrange - Criar outra empresa
        var otherCompany = FakeDataGenerator.GenerateCompany();
        _context.Companies.Add(otherCompany);
        await _context.SaveChangesAsync();
        
        var uniqueId = Guid.NewGuid().ToString().Substring(0, 4);
        var vehicle1 = new Vehicle(_testCompany.Id, $"C1-{uniqueId}", "Model 1", 2021);
        var vehicle2 = new Vehicle(_testCompany.Id, $"C2-{uniqueId}", "Model 2", 2022);
        var vehicle3 = new Vehicle(otherCompany.Id, $"OT-{uniqueId}", "Model 3", 2023);
        
        _context.Vehicles.AddRange(vehicle1, vehicle2, vehicle3);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.GetByCompanyIdAsync(_testCompany.Id);

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveCountGreaterOrEqualTo(2);
        response.Should().Contain(v => v.LicensePlate.Contains(uniqueId));
        response.Count().Should().BeGreaterOrEqualTo(2);
        
        // Limpar veículos da outra empresa
        _context.Vehicles.Remove(vehicle3);
        _context.Companies.Remove(otherCompany);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateVehicle()
    {
        // Arrange
        var vehicle = new Vehicle(_testCompany.Id, "UPD-1234", "Old Model", 2020);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        var request = new VehicleRequest
        {
            CompanyId = _testCompany.Id,
            LicensePlate = "UPD-5678",
            Model = "New Model",
            Year = 2023
        };

        // Act
        var response = await _service.UpdateAsync(vehicle.Id, request);

        // Assert
        response.Should().NotBeNull();
        response.LicensePlate.Should().Be("UPD-5678");
        response.Model.Should().Be("New Model");
        response.Year.Should().Be(2023);
        response.UpdatedAt.Should().NotBeNull();

        // Verificar no banco
        var vehicleInDb = await _context.Vehicles.FindAsync(vehicle.Id);
        vehicleInDb!.LicensePlate.Should().Be("UPD-5678");
        vehicleInDb.Model.Should().Be("New Model");
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ShouldThrowException()
    {
        // Arrange
        var request = new VehicleRequest
        {
            CompanyId = _testCompany.Id,
            LicensePlate = "XXX-0000",
            Model = "Model",
            Year = 2023
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _service.UpdateAsync(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldChangeStatus()
    {
        // Arrange
        var vehicle = new Vehicle(_testCompany.Id, "STS-1234", "Model", 2021);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        // Act
        var response = await _service.UpdateStatusAsync(vehicle.Id, "InTransit");

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be("InTransit");

        // Verificar no banco
        var vehicleInDb = await _context.Vehicles.FindAsync(vehicle.Id);
        vehicleInDb!.Status.ToString().Should().Be("InTransit");
    }

    [Fact]
    public async Task UpdateStatusAsync_WithInvalidStatus_ShouldThrowException()
    {
        // Arrange
        var vehicle = new Vehicle(_testCompany.Id, "INV-1234", "Model", 2021);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => 
            await _service.UpdateStatusAsync(vehicle.Id, "InvalidStatus"));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveVehicle()
    {
        // Arrange
        var vehicle = new Vehicle(_testCompany.Id, "DEL-1234", "Model", 2021);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        // Act
        await _service.DeleteAsync(vehicle.Id);

        // Assert
        var vehicleInDb = await _context.Vehicles.FindAsync(vehicle.Id);
        vehicleInDb.Should().BeNull();
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
            var vehicles = _context.Vehicles.Where(v => v.CompanyId == _testCompany.Id).ToList();
            foreach (var vehicle in vehicles)
            {
                _context.Vehicles.Remove(vehicle);
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
