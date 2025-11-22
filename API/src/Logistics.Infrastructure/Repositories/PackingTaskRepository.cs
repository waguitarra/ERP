using Logistics.Domain.Entities;
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

    public async Task<IEnumerable<PackingTask>> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.PackingTasks
            .Include(t => t.Packages)
            .Where(t => t.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PackingTask>> GetAssignedTasksAsync(Guid userId)
    {
        return await _context.PackingTasks
            .Include(t => t.Order)
            .Where(t => t.AssignedTo == userId)
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
}
