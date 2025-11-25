using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class OrderStatusRepository : BaseRepository<OrderStatusConfig>, IOrderStatusRepository
{
    public OrderStatusRepository(LogisticsDbContext context) : base(context)
    {
    }

    public async Task<OrderStatusConfig?> GetByCodeAsync(string code)
    {
        return await _context.OrderStatusConfigs
            .FirstOrDefaultAsync(x => x.Code == code && x.IsActive);
    }

    public async Task<IEnumerable<OrderStatusConfig>> GetAllActiveAsync()
    {
        return await _context.OrderStatusConfigs
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
    }
}
