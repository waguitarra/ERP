using Logistics.Application.DTOs.PutawayTask;

namespace Logistics.Application.Interfaces;

public interface IPutawayTaskService
{
    Task<PutawayTaskResponse> CreateAsync(CreatePutawayTaskRequest request);
    Task<PutawayTaskResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<PutawayTaskResponse>> GetAllAsync();
    Task<IEnumerable<PutawayTaskResponse>> GetByReceiptIdAsync(Guid receiptId);
    Task AssignTaskAsync(Guid taskId, Guid userId);
    Task StartTaskAsync(Guid taskId);
    Task CompleteTaskAsync(Guid taskId);
}
