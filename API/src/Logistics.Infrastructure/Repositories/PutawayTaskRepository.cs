using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class PutawayTaskRepository : IPutawayTaskRepository
{
    private readonly LogisticsDbContext _context;

    public PutawayTaskRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<PutawayTask?> GetByIdAsync(Guid id)
    {
        return await _context.PutawayTasks
            .Include(t => t.Receipt)
            .Include(t => t.Product)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<PutawayTask>> GetByReceiptIdAsync(Guid receiptId)
    {
        return await _context.PutawayTasks
            .Include(t => t.Product)
            .Where(t => t.ReceiptId == receiptId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PutawayTask>> GetPendingTasksAsync(Guid warehouseId)
    {
        return await _context.PutawayTasks
            .Include(t => t.Product)
            .Include(t => t.ToLocation)
            .Where(t => t.Status == Domain.Enums.WMSTaskStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<PutawayTask>> GetAssignedTasksAsync(Guid userId)
    {
        return await _context.PutawayTasks
            .Include(t => t.Product)
            .Where(t => t.AssignedTo == userId)
            .ToListAsync();
    }

    public async Task AddAsync(PutawayTask task)
    {
        await _context.PutawayTasks.AddAsync(task);
    }

    public async Task UpdateAsync(PutawayTask task)
    {
        _context.PutawayTasks.Update(task);
    }
}
