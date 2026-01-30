using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class PickingTaskRepository : IPickingTaskRepository
{
    private readonly LogisticsDbContext _context;

    public PickingTaskRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<PickingTask?> GetByIdAsync(Guid id)
    {
        return await _context.PickingTasks.FindAsync(id);
    }

    public async Task<PickingTask?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.PickingTasks
            .Include(t => t.PickingWave)
            .Include(t => t.Order)
            .Include(t => t.Lines)
                .ThenInclude(l => l.Product)
            .Include(t => t.Lines)
                .ThenInclude(l => l.Location)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<PickingTask>> GetAllAsync()
    {
        return await _context.PickingTasks
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PickingTask>> GetAllWithDetailsAsync()
    {
        return await _context.PickingTasks
            .Include(t => t.PickingWave)
            .Include(t => t.Order)
            .Include(t => t.Lines)
                .ThenInclude(l => l.Product)
            .Include(t => t.Lines)
                .ThenInclude(l => l.Location)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PickingTask>> GetByWaveIdAsync(Guid waveId)
    {
        return await _context.PickingTasks
            .Include(t => t.PickingWave)
            .Include(t => t.Order)
            .Include(t => t.Lines)
            .Where(t => t.PickingWaveId == waveId)
            .OrderByDescending(t => t.Priority)
            .ToListAsync();
    }

    public async Task<IEnumerable<PickingTask>> GetByStatusAsync(int status)
    {
        return await _context.PickingTasks
            .Include(t => t.PickingWave)
            .Include(t => t.Order)
            .Include(t => t.Lines)
            .Where(t => (int)t.Status == status)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PickingTask>> GetPendingAsync()
    {
        return await GetByStatusAsync((int)WMSTaskStatus.Pending);
    }

    public async Task<IEnumerable<PickingTask>> GetInProgressAsync()
    {
        return await _context.PickingTasks
            .Include(t => t.PickingWave)
            .Include(t => t.Order)
            .Include(t => t.Lines)
            .Where(t => t.Status == WMSTaskStatus.InProgress || t.Status == WMSTaskStatus.Assigned)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(PickingTask task)
    {
        await _context.PickingTasks.AddAsync(task);
    }

    public Task UpdateAsync(PickingTask task)
    {
        _context.PickingTasks.Update(task);
        return Task.CompletedTask;
    }
}
