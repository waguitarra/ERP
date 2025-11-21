using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
{
    public InventoryRepository(LogisticsDbContext context) : base(context) { }
    public async Task<IEnumerable<Inventory>> GetByProductIdAsync(Guid productId) =>
        await _context.Inventories.Where(i => i.ProductId == productId).ToListAsync();
    public async Task<Inventory?> GetByProductAndLocationAsync(Guid productId, Guid storageLocationId) =>
        await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId && i.StorageLocationId == storageLocationId);
}
