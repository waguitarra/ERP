using Logistics.Application.DTOs.PickingWave;

namespace Logistics.Application.Interfaces;

public interface IPickingWaveService
{
    Task<PickingWaveResponse> CreateAsync(CreatePickingWaveRequest request);
    Task<PickingWaveResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<PickingWaveResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<PickingWaveResponse>> GetAllAsync();
    Task ReleaseAsync(Guid id);
}
