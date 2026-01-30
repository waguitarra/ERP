using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class PickingLineRepository : IPickingLineRepository
{
    private readonly LogisticsDbContext _context;

    public PickingLineRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<PickingLine?> GetByIdAsync(Guid id)
    {
        return await _context.PickingLines.FindAsync(id);
    }

    public async Task<PickingLine?> GetByIdWithTaskAsync(Guid id, Guid taskId)
    {
        return await _context.PickingLines
            .Include(l => l.PickingTask)
            .FirstOrDefaultAsync(l => l.Id == id && l.PickingTaskId == taskId);
    }

    public async Task<IEnumerable<PickingLine>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.PickingLines
            .Include(l => l.Product)
            .Include(l => l.Location)
            .Where(l => l.PickingTaskId == taskId)
            .ToListAsync();
    }

    public Task UpdateAsync(PickingLine line)
    {
        _context.PickingLines.Update(line);
        return Task.CompletedTask;
    }
}
