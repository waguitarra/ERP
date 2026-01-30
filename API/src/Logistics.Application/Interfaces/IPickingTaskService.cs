using Logistics.Application.DTOs.PickingTask;

namespace Logistics.Application.Interfaces;

public interface IPickingTaskService
{
    Task<PickingTaskResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<PickingTaskResponse>> GetAllAsync();
    Task<IEnumerable<PickingTaskResponse>> GetByWaveIdAsync(Guid waveId);
    Task<IEnumerable<PickingTaskResponse>> GetByStatusAsync(int status);
    Task<IEnumerable<PickingTaskResponse>> GetPendingAsync();
    Task<IEnumerable<PickingTaskResponse>> GetInProgressAsync();
    Task AssignTaskAsync(Guid taskId, Guid userId);
    Task StartTaskAsync(Guid taskId);
    Task CompleteTaskAsync(Guid taskId);
    Task CancelTaskAsync(Guid taskId);
    Task PickLineAsync(Guid taskId, Guid lineId, decimal quantityPicked);
}
