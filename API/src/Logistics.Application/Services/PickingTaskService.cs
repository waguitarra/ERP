using Logistics.Application.DTOs.PickingTask;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;
using Serilog;

namespace Logistics.Application.Services;

public class PickingTaskService : IPickingTaskService
{
    private readonly IPickingTaskRepository _taskRepository;
    private readonly IPickingLineRepository _lineRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PickingTaskService(
        IPickingTaskRepository taskRepository,
        IPickingLineRepository lineRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _lineRepository = lineRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        Log.Information("[PickingTaskService] Construtor criado");
    }

    public async Task<PickingTaskResponse> GetByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdWithDetailsAsync(id);
        if (task == null)
            throw new KeyNotFoundException("Picking Task não encontrada");

        return await MapToResponseAsync(task);
    }

    public async Task<IEnumerable<PickingTaskResponse>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllWithDetailsAsync();
        var responses = new List<PickingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task<IEnumerable<PickingTaskResponse>> GetByWaveIdAsync(Guid waveId)
    {
        var tasks = await _taskRepository.GetByWaveIdAsync(waveId);
        var responses = new List<PickingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task<IEnumerable<PickingTaskResponse>> GetByStatusAsync(int status)
    {
        var tasks = await _taskRepository.GetByStatusAsync(status);
        var responses = new List<PickingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task<IEnumerable<PickingTaskResponse>> GetPendingAsync()
    {
        var tasks = await _taskRepository.GetPendingAsync();
        var responses = new List<PickingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task<IEnumerable<PickingTaskResponse>> GetInProgressAsync()
    {
        var tasks = await _taskRepository.GetInProgressAsync();
        var responses = new List<PickingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task AssignTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Picking Task não encontrada");

        task.AssignTo(userId);
        await _taskRepository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
        Log.Information("[PickingTaskService] Task {TaskId} atribuída ao usuário {UserId}", taskId, userId);
    }

    public async Task StartTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Picking Task não encontrada");

        task.Start();
        await _taskRepository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
        Log.Information("[PickingTaskService] Task {TaskId} iniciada", taskId);
    }

    public async Task CompleteTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Picking Task não encontrada");

        task.Complete();
        await _taskRepository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
        Log.Information("[PickingTaskService] Task {TaskId} completada", taskId);
    }

    public async Task CancelTaskAsync(Guid taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new KeyNotFoundException("Picking Task não encontrada");

        task.Cancel();
        await _taskRepository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
        Log.Information("[PickingTaskService] Task {TaskId} cancelada", taskId);
    }

    public async Task PickLineAsync(Guid taskId, Guid lineId, decimal quantityPicked)
    {
        var line = await _lineRepository.GetByIdWithTaskAsync(lineId, taskId);
        if (line == null)
            throw new KeyNotFoundException("Picking Line não encontrada");

        line.Pick(quantityPicked, null);
        await _lineRepository.UpdateAsync(line);
        await _unitOfWork.CommitAsync();
        Log.Information("[PickingTaskService] Line {LineId} picked: {Quantity}", lineId, quantityPicked);
    }

    private async Task<PickingTaskResponse> MapToResponseAsync(PickingTask task)
    {
        string? userName = null;
        if (task.AssignedTo.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(task.AssignedTo.Value);
            userName = user?.Name;
        }

        return new PickingTaskResponse
        {
            Id = task.Id,
            TaskNumber = task.TaskNumber,
            PickingWaveId = task.PickingWaveId,
            WaveNumber = task.PickingWave?.WaveNumber,
            OrderId = task.OrderId,
            OrderNumber = task.Order?.OrderNumber,
            Priority = (int)task.Priority,
            PriorityName = task.Priority.ToString(),
            Status = (int)task.Status,
            StatusName = task.Status.ToString(),
            AssignedTo = task.AssignedTo,
            AssignedToName = userName,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            TotalLines = task.Lines?.Count ?? 0,
            CompletedLines = task.Lines?.Count(l => l.Status == PickingLineStatus.Picked) ?? 0,
            TotalQuantityToPick = task.Lines?.Sum(l => l.QuantityToPick) ?? 0,
            TotalQuantityPicked = task.Lines?.Sum(l => l.QuantityPicked) ?? 0,
            Lines = task.Lines?.Select(l => new PickingLineResponse
            {
                Id = l.Id,
                ProductId = l.ProductId,
                ProductName = l.Product?.Name ?? "",
                ProductSku = l.Product?.SKU ?? "",
                LocationId = l.LocationId,
                LocationCode = l.Location?.Code ?? "",
                LotId = l.LotId,
                LotNumber = null,
                SerialNumber = l.SerialNumber,
                QuantityToPick = l.QuantityToPick,
                QuantityPicked = l.QuantityPicked,
                Status = (int)l.Status,
                StatusName = l.Status.ToString(),
                PickedBy = l.PickedBy,
                PickedAt = l.PickedAt
            }).ToList() ?? new List<PickingLineResponse>()
        };
    }
}
