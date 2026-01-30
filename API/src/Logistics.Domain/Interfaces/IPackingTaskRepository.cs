using Logistics.Domain.Entities;
using Logistics.Domain.Enums;

namespace Logistics.Domain.Interfaces;

public interface IPackingTaskRepository
{
    Task<PackingTask?> GetByIdAsync(Guid id);
    Task<IEnumerable<PackingTask>> GetAllAsync();
    Task<IEnumerable<PackingTask>> GetAllWithDetailsAsync();
    Task<IEnumerable<PackingTask>> GetByOrderIdAsync(Guid orderId);
    Task<IEnumerable<PackingTask>> GetByStatusAsync(WMSTaskStatus status);
    Task<IEnumerable<PackingTask>> GetAssignedTasksAsync(Guid userId);
    Task<IEnumerable<PackingTask>> GetPendingTasksAsync();
    Task<IEnumerable<PackingTask>> GetInProgressTasksAsync();
    Task AddAsync(PackingTask task);
    Task UpdateAsync(PackingTask task);
    Task DeleteAsync(PackingTask task);
}
