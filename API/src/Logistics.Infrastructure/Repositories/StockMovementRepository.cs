using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class StockMovementRepository : BaseRepository<StockMovement>, IStockMovementRepository
{
    public StockMovementRepository(LogisticsDbContext context) : base(context) { }
    public async Task<IEnumerable<StockMovement>> GetByProductIdAsync(Guid productId) =>
        await _context.StockMovements.Where(sm => sm.ProductId == productId).OrderByDescending(sm => sm.MovementDate).ToListAsync();
}
