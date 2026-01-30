using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IPickingTaskRepository
{
    Task<PickingTask?> GetByIdAsync(Guid id);
    Task<PickingTask?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<PickingTask>> GetAllAsync();
    Task<IEnumerable<PickingTask>> GetAllWithDetailsAsync();
    Task<IEnumerable<PickingTask>> GetByWaveIdAsync(Guid waveId);
    Task<IEnumerable<PickingTask>> GetByStatusAsync(int status);
    Task<IEnumerable<PickingTask>> GetPendingAsync();
    Task<IEnumerable<PickingTask>> GetInProgressAsync();
    Task AddAsync(PickingTask task);
    Task UpdateAsync(PickingTask task);
}
