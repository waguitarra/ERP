using Logistics.Application.DTOs.PutawayTask;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class PutawayTaskService : IPutawayTaskService
{
    private readonly IPutawayTaskRepository _repository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PutawayTaskService(
        IPutawayTaskRepository repository,
        IReceiptRepository receiptRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _receiptRepository = receiptRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PutawayTaskResponse> CreateAsync(CreatePutawayTaskRequest request)
    {
        if (await _receiptRepository.GetByIdAsync(request.ReceiptId) == null)
            throw new KeyNotFoundException("Receipt não encontrado");

        if (await _productRepository.GetByIdAsync(request.ProductId) == null)
            throw new KeyNotFoundException("Produto não encontrado");

        var task = new PutawayTask(
            request.TaskNumber,
            request.ReceiptId,
            request.ProductId,
            request.Quantity,
            request.FromLocationId,
            request.ToLocationId
        );

        if (request.LotId.HasValue)
            task.SetLot(request.LotId.Value);

        await _repository.AddAsync(task);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(task.Id);
    }

    public async Task<PutawayTaskResponse> GetByIdAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null) throw new KeyNotFoundException("Tarefa não encontrada");
        return MapToResponse(task);
    }

    public async Task<IEnumerable<PutawayTaskResponse>> GetByReceiptIdAsync(Guid receiptId)
    {
        var tasks = await _repository.GetByReceiptIdAsync(receiptId);
        return tasks.Select(MapToResponse);
    }

    public async Task AssignTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null) throw new KeyNotFoundException("Tarefa não encontrada");

        task.AssignTo(userId);
        await _repository.UpdateAsync(task);
        await _unitOfWork.CommitAsync();
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

    private static PutawayTaskResponse MapToResponse(PutawayTask task)
    {
        return new PutawayTaskResponse(
            task.Id,
            task.TaskNumber,
            task.ReceiptId,
            task.ProductId,
            task.Product?.Name ?? "",
            task.Quantity,
            task.FromLocationId,
            task.ToLocationId,
            task.Status,
            task.AssignedTo,
            task.CompletedAt,
            task.CreatedAt
        );
    }
}
