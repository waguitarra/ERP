using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Logistics.Infrastructure.Data;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataSeederController : ControllerBase
{
    private readonly LogisticsDbContext _context;
    
    public DataSeederController(LogisticsDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("populate-addresses")]
    public async Task<IActionResult> PopulateAddresses()
    {
        // Atualizar Warehouses
        var warehouses = await _context.Warehouses.ToListAsync();
        foreach (var wh in warehouses)
        {
            if (wh.Code.Contains("001") || wh.Name.Contains("Central"))
            {
                _context.Database.ExecuteSqlRaw(
                    @"UPDATE `Warehouses` SET 
                        `Address` = 'Calle de Alcalá, 123',
                        `City` = 'Madrid',
                        `State` = 'Madrid',
                        `ZipCode` = '28009',
                        `Country` = 'España',
                        `Latitude` = 40.4168,
                        `Longitude` = -3.7038,
                        `UpdatedAt` = NOW()
                    WHERE `Id` = {0}", wh.Id);
            }
            else if (wh.Code.Contains("002"))
            {
                _context.Database.ExecuteSqlRaw(
                    @"UPDATE `Warehouses` SET 
                        `Address` = 'Carrer de Provença, 456',
                        `City` = 'Barcelona',
                        `State` = 'Cataluña',
                        `ZipCode` = '08025',
                        `Country` = 'España',
                        `Latitude` = 41.3851,
                        `Longitude` = 2.1734,
                        `UpdatedAt` = NOW()
                    WHERE `Id` = {0}", wh.Id);
            }
            else
            {
                _context.Database.ExecuteSqlRaw(
                    @"UPDATE `Warehouses` SET 
                        `Address` = 'Avenida Diagonal, 300',
                        `City` = 'Valencia',
                        `State` = 'Valencia',
                        `ZipCode` = '46001',
                        `Country` = 'España',
                        `Latitude` = 39.4699,
                        `Longitude` = -0.3763,
                        `UpdatedAt` = NOW()
                    WHERE `Id` = {0}", wh.Id);
            }
        }
        
        // Atualizar Customers
        var customers = await _context.Customers.ToListAsync();
        var cities = new[] { "Madrid", "Barcelona", "Valencia", "Sevilla" };
        var random = new Random();
        
        for (int i = 0; i < customers.Count; i++)
        {
            var customer = customers[i];
            var city = cities[i % cities.Length];
            var state = city == "Madrid" ? "Madrid" : 
                       city == "Barcelona" ? "Cataluña" :
                       city == "Valencia" ? "Valencia" : "Andalucía";
            var zipPrefix = city == "Madrid" ? "280" :
                           city == "Barcelona" ? "080" :
                           city == "Valencia" ? "460" : "410";
            var lat = city == "Madrid" ? 40.4168 :
                     city == "Barcelona" ? 41.3851 :
                     city == "Valencia" ? 39.4699 : 37.3886;
            var lon = city == "Madrid" ? -3.7038 :
                     city == "Barcelona" ? 2.1734 :
                     city == "Valencia" ? -0.3763 : -5.9823;
            
            _context.Database.ExecuteSqlRaw(
                @"UPDATE `Customers` SET 
                    `Address` = {1},
                    `City` = {2},
                    `State` = {3},
                    `ZipCode` = {4},
                    `Country` = 'España',
                    `Latitude` = {5},
                    `Longitude` = {6},
                    `UpdatedAt` = NOW()
                WHERE `Id` = {0}", 
                customer.Id,
                $"Gran Vía, {random.Next(1, 100)}",
                city,
                state,
                $"{zipPrefix}{random.Next(10, 99):00}",
                lat + (random.NextDouble() * 0.1 - 0.05),
                lon + (random.NextDouble() * 0.1 - 0.05));
        }
        
        var whCount = await _context.Warehouses.CountAsync(w => w.City != null);
        var custCount = await _context.Customers.CountAsync(c => c.City != null);
        
        return Ok(new 
        { 
            message = "Endereços atualizados com sucesso",
            warehousesUpdated = whCount,
            customersUpdated = custCount
        });
    }
    
    [HttpGet("verify-addresses")]
    public async Task<IActionResult> VerifyAddresses()
    {
        var warehouses = await _context.Warehouses
            .Select(w => new { w.Name, w.Code, w.City, w.State, w.ZipCode, w.Country })
            .Take(5)
            .ToListAsync();
            
        var customers = await _context.Customers
            .Select(c => new { c.Name, c.Document, c.City, c.State, c.ZipCode, c.Address })
            .Take(5)
            .ToListAsync();
            
        return Ok(new { warehouses, customers });
    }
}
