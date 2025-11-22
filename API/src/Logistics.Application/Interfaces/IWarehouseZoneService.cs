using Logistics.Application.DTOs.WarehouseZone;

namespace Logistics.Application.Interfaces;

public interface IWarehouseZoneService
{
    Task<WarehouseZoneResponse> CreateAsync(CreateWarehouseZoneRequest request);
    Task<WarehouseZoneResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<WarehouseZoneResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<WarehouseZoneResponse>> GetAllAsync();
}
