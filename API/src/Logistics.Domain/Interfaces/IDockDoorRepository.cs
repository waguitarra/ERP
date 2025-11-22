using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IDockDoorRepository
{
    Task<DockDoor?> GetByIdAsync(Guid id);
    Task<IEnumerable<DockDoor>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<DockDoor>> GetAvailableAsync(Guid warehouseId);
    Task<IEnumerable<DockDoor>> GetAllAsync();
    Task AddAsync(DockDoor dockDoor);
    Task UpdateAsync(DockDoor dockDoor);
    Task DeleteAsync(Guid id);
}
