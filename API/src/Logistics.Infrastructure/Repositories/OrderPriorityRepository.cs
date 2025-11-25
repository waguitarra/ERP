using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class OrderPriorityRepository : BaseRepository<OrderPriorityConfig>, IOrderPriorityRepository
{
    public OrderPriorityRepository(LogisticsDbContext context) : base(context)
    {
    }

    public async Task<OrderPriorityConfig?> GetByCodeAsync(string code)
    {
        return await _context.OrderPriorityConfigs
            .FirstOrDefaultAsync(x => x.Code == code && x.IsActive);
    }

    public async Task<IEnumerable<OrderPriorityConfig>> GetAllActiveAsync()
    {
        return await _context.OrderPriorityConfigs
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
    }
}
