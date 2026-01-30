using Logistics.Application.DTOs.PackingTask;
using Logistics.Domain.Enums;

namespace Logistics.Application.Interfaces;

public interface IPackingTaskService
{
    Task<PackingTaskResponse> CreateAsync(CreatePackingTaskRequest request);
    Task<PackingTaskResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<PackingTaskResponse>> GetAllAsync();
    Task<IEnumerable<PackingTaskResponse>> GetByOrderIdAsync(Guid orderId);
    Task<IEnumerable<PackingTaskResponse>> GetByStatusAsync(WMSTaskStatus status);
    Task<IEnumerable<PackingTaskResponse>> GetPendingAsync();
    Task<IEnumerable<PackingTaskResponse>> GetInProgressAsync();
    Task StartTaskAsync(Guid taskId);
    Task CompleteTaskAsync(Guid taskId);
    Task CancelTaskAsync(Guid taskId);
    Task DeleteAsync(Guid taskId);
}
