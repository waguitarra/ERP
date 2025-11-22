using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IPackingTaskRepository
{
    Task<PackingTask?> GetByIdAsync(Guid id);
    Task<IEnumerable<PackingTask>> GetByOrderIdAsync(Guid orderId);
    Task<IEnumerable<PackingTask>> GetAssignedTasksAsync(Guid userId);
    Task AddAsync(PackingTask task);
    Task UpdateAsync(PackingTask task);
}
