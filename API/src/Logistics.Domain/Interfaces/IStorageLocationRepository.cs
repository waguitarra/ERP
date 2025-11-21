using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IStorageLocationRepository : IBaseRepository<StorageLocation>
{
    Task<IEnumerable<StorageLocation>> GetByWarehouseIdAsync(Guid warehouseId);
}
