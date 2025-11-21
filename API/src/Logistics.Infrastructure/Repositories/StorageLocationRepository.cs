using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class StorageLocationRepository : BaseRepository<StorageLocation>, IStorageLocationRepository
{
    public StorageLocationRepository(LogisticsDbContext context) : base(context) { }
    public async Task<IEnumerable<StorageLocation>> GetByWarehouseIdAsync(Guid warehouseId) =>
        await _context.StorageLocations.Where(sl => sl.WarehouseId == warehouseId && sl.IsActive).ToListAsync();
}
