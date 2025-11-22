using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class PickingWaveRepository : IPickingWaveRepository
{
    private readonly LogisticsDbContext _context;

    public PickingWaveRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<PickingWave?> GetByIdAsync(Guid id)
    {
        return await _context.PickingWaves
            .Include(w => w.Tasks)
            .ThenInclude(t => t.Lines)
            .Include(w => w.Warehouse)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<PickingWave?> GetByWaveNumberAsync(string waveNumber)
    {
        return await _context.PickingWaves
            .FirstOrDefaultAsync(w => w.WaveNumber == waveNumber);
    }

    public async Task<IEnumerable<PickingWave>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _context.PickingWaves
            .Include(w => w.Tasks)
            .Where(w => w.WarehouseId == warehouseId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PickingWave>> GetAllAsync()
    {
        return await _context.PickingWaves
            .Include(w => w.Tasks)
            .ToListAsync();
    }

    public async Task AddAsync(PickingWave wave)
    {
        await _context.PickingWaves.AddAsync(wave);
    }

    public Task UpdateAsync(PickingWave wave)
    {
        _context.PickingWaves.Update(wave);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var wave = await GetByIdAsync(id);
        if (wave != null)
            _context.PickingWaves.Remove(wave);
    }
}
