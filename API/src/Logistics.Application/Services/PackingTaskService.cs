using Logistics.Application.DTOs.PackingTask;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class PackingTaskService : IPackingTaskService
{
    private readonly IPackingTaskRepository _repository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PackingTaskService(
        IPackingTaskRepository repository,
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PackingTaskResponse> CreateAsync(CreatePackingTaskRequest request)
    {
        if (await _orderRepository.GetByIdAsync(request.OrderId) == null)
            throw new KeyNotFoundException("Pedido não encontrado");

        var task = new PackingTask(
            request.TaskNumber,
            request.OrderId,
            request.AssignedTo
        );

        await _repository.AddAsync(task);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(task.Id);
    }

    public async Task<PackingTaskResponse> GetByIdAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null) throw new KeyNotFoundException("Tarefa não encontrada");
        return await MapToResponseAsync(task);
    }

    public async Task<IEnumerable<PackingTaskResponse>> GetAllAsync()
    {
        var tasks = await _repository.GetAllWithDetailsAsync();
        var responses = new List<PackingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task<IEnumerable<PackingTaskResponse>> GetByOrderIdAsync(Guid orderId)
    {
        var tasks = await _repository.GetByOrderIdAsync(orderId);
        var responses = new List<PackingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task<IEnumerable<PackingTaskResponse>> GetByStatusAsync(WMSTaskStatus status)
    {
        var tasks = await _repository.GetByStatusAsync(status);
        var responses = new List<PackingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task<IEnumerable<PackingTaskResponse>> GetPendingAsync()
    {
        var tasks = await _repository.GetPendingTasksAsync();
        var responses = new List<PackingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task<IEnumerable<PackingTaskResponse>> GetInProgressAsync()
    {
        var tasks = await _repository.GetInProgressTasksAsync();
        var responses = new List<PackingTaskResponse>();
        foreach (var task in tasks)
        {
            responses.Add(await MapToResponseAsync(task));
        }
        return responses;
    }

    public async Task StartTaskAsync(Guid taskId)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null) throw new KeyNotFoundException("Tarefa não encontrada");

        task.Start();
        await _repository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
    }

    public async Task CompleteTaskAsync(Guid taskId)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null) throw new KeyNotFoundException("Tarefa não encontrada");

        task.Complete();
        await _repository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
    }

    public async Task CancelTaskAsync(Guid taskId)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null) throw new KeyNotFoundException("Tarefa não encontrada");

        task.Cancel();
        await _repository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid taskId)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null) throw new KeyNotFoundException("Tarefa não encontrada");

        await _repository.DeleteAsync(task);
        await _unitOfWork.CommitAsync();
    }

    private async Task<PackingTaskResponse> MapToResponseAsync(PackingTask task)
    {
        var assignedUser = await _userRepository.GetByIdAsync(task.AssignedTo);
        
        return new PackingTaskResponse(
            task.Id,
            task.TaskNumber,
            task.OrderId,
            task.Order?.OrderNumber ?? "",
            (int)task.Status,
            task.Status.ToString(),
            task.AssignedTo,
            assignedUser?.Name ?? "",
            task.Packages?.Count ?? 0,
            task.CompletedAt,
            task.CreatedAt,
            task.UpdatedAt
        );
    }
}
