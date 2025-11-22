using Logistics.Application.DTOs.PackingTask;

namespace Logistics.Application.Interfaces;

public interface IPackingTaskService
{
    Task<PackingTaskResponse> CreateAsync(CreatePackingTaskRequest request);
    Task<PackingTaskResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<PackingTaskResponse>> GetByOrderIdAsync(Guid orderId);
    Task StartTaskAsync(Guid taskId);
    Task CompleteTaskAsync(Guid taskId);
}
