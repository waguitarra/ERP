using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IWarehouseZoneRepository
{
    Task<WarehouseZone?> GetByIdAsync(Guid id);
    Task<IEnumerable<WarehouseZone>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<WarehouseZone>> GetAllAsync();
    Task AddAsync(WarehouseZone zone);
    Task UpdateAsync(WarehouseZone zone);
    Task DeleteAsync(Guid id);
}
