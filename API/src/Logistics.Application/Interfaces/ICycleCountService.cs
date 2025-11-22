using Logistics.Application.DTOs.CycleCount;

namespace Logistics.Application.Interfaces;

public interface ICycleCountService
{
    Task<CycleCountResponse> CreateAsync(CreateCycleCountRequest request);
    Task<CycleCountResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<CycleCountResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task CompleteAsync(Guid id);
}
