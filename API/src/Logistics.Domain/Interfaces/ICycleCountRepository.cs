using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface ICycleCountRepository
{
    Task<CycleCount?> GetByIdAsync(Guid id);
    Task<IEnumerable<CycleCount>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<CycleCount>> GetInProgressAsync();
    Task AddAsync(CycleCount cycleCount);
    Task UpdateAsync(CycleCount cycleCount);
}
