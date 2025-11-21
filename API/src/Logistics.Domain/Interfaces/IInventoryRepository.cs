using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IInventoryRepository : IBaseRepository<Inventory>
{
    Task<IEnumerable<Inventory>> GetByProductIdAsync(Guid productId);
    Task<Inventory?> GetByProductAndLocationAsync(Guid productId, Guid storageLocationId);
}
