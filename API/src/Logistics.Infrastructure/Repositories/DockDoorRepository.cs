using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Domain.Enums;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class DockDoorRepository : IDockDoorRepository
{
    private readonly LogisticsDbContext _context;

    public DockDoorRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<DockDoor?> GetByIdAsync(Guid id)
    {
        return await _context.DockDoors
            .Include(d => d.Warehouse)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<DockDoor>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _context.DockDoors
            .Where(d => d.WarehouseId == warehouseId && d.IsActive)
            .OrderBy(d => d.DoorNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<DockDoor>> GetAvailableAsync(Guid warehouseId)
    {
        return await _context.DockDoors
            .Where(d => d.WarehouseId == warehouseId && d.IsActive && d.Status == DockDoorStatus.Available)
            .OrderBy(d => d.DoorNumber)
            .ToListAsync();
    }

    public async Task<IEnumerable<DockDoor>> GetAllAsync()
    {
        return await _context.DockDoors
            .Include(d => d.Warehouse)
            .ToListAsync();
    }

    public async Task AddAsync(DockDoor dockDoor)
    {
        await _context.DockDoors.AddAsync(dockDoor);
    }

    public Task UpdateAsync(DockDoor dockDoor)
    {
        _context.DockDoors.Update(dockDoor);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var dockDoor = await GetByIdAsync(id);
        if (dockDoor != null)
            _context.DockDoors.Remove(dockDoor);
    }
}
