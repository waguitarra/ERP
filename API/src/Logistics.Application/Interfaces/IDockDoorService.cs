using Logistics.Application.DTOs.DockDoor;

namespace Logistics.Application.Interfaces;

public interface IDockDoorService
{
    Task<DockDoorResponse> CreateAsync(CreateDockDoorRequest request);
    Task<DockDoorResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<DockDoorResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<DockDoorResponse>> GetAvailableAsync(Guid warehouseId);
    Task<IEnumerable<DockDoorResponse>> GetAllAsync();
}
