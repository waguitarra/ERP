using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class WarehouseZoneRepository : IWarehouseZoneRepository
{
    private readonly LogisticsDbContext _context;

    public WarehouseZoneRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseZone?> GetByIdAsync(Guid id)
    {
        return await _context.WarehouseZones
            .Include(z => z.Warehouse)
            .FirstOrDefaultAsync(z => z.Id == id);
    }

    public async Task<IEnumerable<WarehouseZone>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _context.WarehouseZones
            .Where(z => z.WarehouseId == warehouseId)
            .OrderBy(z => z.ZoneName)
            .ToListAsync();
    }

    public async Task<IEnumerable<WarehouseZone>> GetAllAsync()
    {
        return await _context.WarehouseZones
            .Include(z => z.Warehouse)
            .ToListAsync();
    }

    public async Task AddAsync(WarehouseZone zone)
    {
        await _context.WarehouseZones.AddAsync(zone);
    }

    public Task UpdateAsync(WarehouseZone zone)
    {
        _context.WarehouseZones.Update(zone);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var zone = await GetByIdAsync(id);
        if (zone != null)
            _context.WarehouseZones.Remove(zone);
    }
}
