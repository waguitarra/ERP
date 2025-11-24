using Logistics.Application.DTOs.PutawayTask;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Serilog;

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
        Log.Information("[PutawayTaskService] CONSTRUTOR criado");
    }

    public async Task<PutawayTaskResponse> CreateAsync(CreatePutawayTaskRequest request)
    {
        Log.Information("[PutawayTaskService] ========== CREATE ASYNC INICIO ==========");
        Log.Information("[PutawayTaskService] TaskNumber: {TaskNumber}", request.TaskNumber);
        Log.Information("[PutawayTaskService] ReceiptId: {ReceiptId}", request.ReceiptId);
        Log.Information("[PutawayTaskService] ProductId: {ProductId}", request.ProductId);
        Log.Information("[PutawayTaskService] Quantity: {Quantity}", request.Quantity);
        
        if (await _receiptRepository.GetByIdAsync(request.ReceiptId) == null)
        {
            Log.Error("[PutawayTaskService] Receipt não encontrado: {ReceiptId}", request.ReceiptId);
            throw new KeyNotFoundException("Receipt não encontrado");
        }
        Log.Information("[PutawayTaskService] Receipt encontrado OK");

        if (await _productRepository.GetByIdAsync(request.ProductId) == null)
        {
            Log.Error("[PutawayTaskService] Produto não encontrado: {ProductId}", request.ProductId);
            throw new KeyNotFoundException("Produto não encontrado");
        }
        Log.Information("[PutawayTaskService] Product encontrado OK");

        var task = new PutawayTask(
            request.TaskNumber,
            request.ReceiptId,
            request.ProductId,
            request.Quantity,
            request.FromLocationId,
            request.ToLocationId
        );
        
        Log.Information("[PutawayTaskService] PutawayTask objeto criado - ID: {TaskId}", task.Id);

        if (request.LotId.HasValue)
        {
            task.SetLot(request.LotId.Value);
            Log.Information("[PutawayTaskService] Lot setado: {LotId}", request.LotId.Value);
        }

        Log.Information("[PutawayTaskService] Chamando Repository.AddAsync...");
        await _repository.AddAsync(task);
        Log.Information("[PutawayTaskService] Repository.AddAsync concluído");
        
        Log.Information("[PutawayTaskService] Chamando UnitOfWork.CommitAsync...");
        var savedCount = await _unitOfWork.CommitAsync();
        Log.Information("[PutawayTaskService] CommitAsync retornou: {SavedCount} registros salvos", savedCount);

        if (savedCount == 0)
        {
            Log.Error("[PutawayTaskService] ERRO: SaveChanges retornou 0 - NADA FOI SALVO!");
        }

        Log.Information("[PutawayTaskService] ========== CREATE ASYNC FIM ==========");
        return MapToResponse(task);
    }

    public async Task<IEnumerable<PutawayTaskResponse>> GetAllAsync()
    {
        var tasks = await _repository.GetAllAsync();
        return tasks.Select(MapToResponse);
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
