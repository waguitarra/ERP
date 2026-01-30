using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class PackingTaskRepository : IPackingTaskRepository
{
    private readonly LogisticsDbContext _context;

    public PackingTaskRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<PackingTask?> GetByIdAsync(Guid id)
    {
        return await _context.PackingTasks
            .Include(t => t.Order)
            .Include(t => t.Packages)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<PackingTask>> GetAllAsync()
    {
        return await _context.PackingTasks
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PackingTask>> GetAllWithDetailsAsync()
    {
        return await _context.PackingTasks
            .Include(t => t.Order)
            .Include(t => t.Packages)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PackingTask>> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.PackingTasks
            .Include(t => t.Packages)
            .Where(t => t.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PackingTask>> GetByStatusAsync(WMSTaskStatus status)
    {
        return await _context.PackingTasks
            .Include(t => t.Order)
            .Include(t => t.Packages)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PackingTask>> GetAssignedTasksAsync(Guid userId)
    {
        return await _context.PackingTasks
            .Include(t => t.Order)
            .Where(t => t.AssignedTo == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PackingTask>> GetPendingTasksAsync()
    {
        return await _context.PackingTasks
            .Include(t => t.Order)
            .Include(t => t.Packages)
            .Where(t => t.Status == WMSTaskStatus.Assigned || t.Status == WMSTaskStatus.Pending)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PackingTask>> GetInProgressTasksAsync()
    {
        return await _context.PackingTasks
            .Include(t => t.Order)
            .Include(t => t.Packages)
            .Where(t => t.Status == WMSTaskStatus.InProgress)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(PackingTask task)
    {
        await _context.PackingTasks.AddAsync(task);
    }

    public async Task UpdateAsync(PackingTask task)
    {
        _context.PackingTasks.Update(task);
    }

    public async Task DeleteAsync(PackingTask task)
    {
        _context.PackingTasks.Remove(task);
    }
}
