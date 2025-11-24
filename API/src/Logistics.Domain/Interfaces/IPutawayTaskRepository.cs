using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IPutawayTaskRepository
{
    Task<IEnumerable<PutawayTask>> GetAllAsync();
    Task<PutawayTask?> GetByIdAsync(Guid id);
    Task<IEnumerable<PutawayTask>> GetByReceiptIdAsync(Guid receiptId);
    Task<IEnumerable<PutawayTask>> GetPendingTasksAsync(Guid warehouseId);
    Task<IEnumerable<PutawayTask>> GetAssignedTasksAsync(Guid userId);
    Task AddAsync(PutawayTask task);
    Task UpdateAsync(PutawayTask task);
}
