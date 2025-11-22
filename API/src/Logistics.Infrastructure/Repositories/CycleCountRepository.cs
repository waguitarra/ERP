using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class CycleCountRepository : ICycleCountRepository
{
    private readonly LogisticsDbContext _context;

    public CycleCountRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<CycleCount?> GetByIdAsync(Guid id)
    {
        return await _context.CycleCounts
            .Include(c => c.Warehouse)
            .Include(c => c.Zone)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<CycleCount>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _context.CycleCounts
            .Where(c => c.WarehouseId == warehouseId)
            .OrderByDescending(c => c.CountDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<CycleCount>> GetInProgressAsync()
    {
        return await _context.CycleCounts
            .Include(c => c.Warehouse)
            .Where(c => c.Status == CycleCountStatus.InProgress)
            .ToListAsync();
    }

    public async Task AddAsync(CycleCount cycleCount)
    {
        await _context.CycleCounts.AddAsync(cycleCount);
    }

    public async Task UpdateAsync(CycleCount cycleCount)
    {
        _context.CycleCounts.Update(cycleCount);
    }
}
