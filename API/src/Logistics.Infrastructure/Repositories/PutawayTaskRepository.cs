using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Logistics.Infrastructure.Repositories;

public class PutawayTaskRepository : IPutawayTaskRepository
{
    private readonly LogisticsDbContext _context;

    public PutawayTaskRepository(LogisticsDbContext context)
    {
        _context = context;
        Log.Information("[PutawayTaskRepository] CONSTRUTOR - DbContext HashCode: {HashCode}", _context.GetHashCode());
    }

    public async Task<IEnumerable<PutawayTask>> GetAllAsync()
    {
        return await _context.PutawayTasks
            .Include(t => t.Product)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .ToListAsync();
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
        Log.Information("[PutawayTaskRepository] ===== AddAsync INICIO =====");
        Log.Information("[PutawayTaskRepository] DbContext HashCode: {HashCode}", _context.GetHashCode());
        Log.Information("[PutawayTaskRepository] Task ID: {TaskId}", task.Id);
        Log.Information("[PutawayTaskRepository] Task Number: {TaskNumber}", task.TaskNumber);
        
        await _context.PutawayTasks.AddAsync(task);
        
        var entries = _context.ChangeTracker.Entries().Count();
        Log.Information("[PutawayTaskRepository] ChangeTracker Entries APÓS Add: {Entries}", entries);
        
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            Log.Information("[PutawayTaskRepository] Entry: {EntityType} - State: {State}", entry.Entity.GetType().Name, entry.State);
        }
        
        Log.Information("[PutawayTaskRepository] ===== AddAsync FIM =====");
    }

    public async Task UpdateAsync(PutawayTask task)
    {
        _context.PutawayTasks.Update(task);
    }
}
