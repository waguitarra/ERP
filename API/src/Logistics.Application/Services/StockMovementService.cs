using Logistics.Application.DTOs.StockMovement;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class StockMovementService : IStockMovementService
{
    private readonly IStockMovementRepository _repository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockMovementService(
        IStockMovementRepository repository,
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IStorageLocationRepository storageLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _storageLocationRepository = storageLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StockMovementResponse> CreateAsync(StockMovementRequest request)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
            throw new KeyNotFoundException($"Produto não encontrado: {request.ProductId}");

        var movement = new StockMovement(
            request.ProductId,
            request.StorageLocationId ?? Guid.Empty,
            request.Type,
            request.Quantity,
            request.Reference,
            request.Notes
        );

        await _repository.AddAsync(movement);

        // Atualizar inventário se necessário
        if (request.StorageLocationId.HasValue)
        {
            var inventory = (await _inventoryRepository.GetByProductIdAsync(request.ProductId))
                .FirstOrDefault(i => i.StorageLocationId == request.StorageLocationId.Value);

            if (inventory != null)
            {
                if (request.Type == Domain.Enums.StockMovementType.Inbound)
                    inventory.AddStock(request.Quantity);
                else
                    inventory.RemoveStock(request.Quantity);
            }
        }

        await _unitOfWork.CommitAsync();

        return await MapToResponseAsync(movement);
    }

    public async Task<StockMovementResponse> GetByIdAsync(Guid id)
    {
        var movement = await _repository.GetByIdAsync(id);
        if (movement == null)
            throw new KeyNotFoundException($"Movimentação não encontrada: {id}");

        return await MapToResponseAsync(movement);
    }

    public async Task<IEnumerable<StockMovementResponse>> GetAllAsync()
    {
        var movements = await _repository.GetAllAsync();
        var responses = new List<StockMovementResponse>();
        
        foreach (var mov in movements)
        {
            responses.Add(await MapToResponseAsync(mov));
        }
        
        return responses;
    }

    public async Task<IEnumerable<StockMovementResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var allMovements = await _repository.GetAllAsync();
        var movements = allMovements.Where(m =>
        {
            var loc = _storageLocationRepository.GetByIdAsync(m.StorageLocationId).Result;
            return loc?.WarehouseId == warehouseId;
        });
        
        var responses = new List<StockMovementResponse>();
        foreach (var mov in movements)
        {
            responses.Add(await MapToResponseAsync(mov));
        }
        
        return responses;
    }

    public async Task<IEnumerable<StockMovementResponse>> GetByProductIdAsync(Guid productId)
    {
        var movements = await _repository.GetByProductIdAsync(productId);
        var responses = new List<StockMovementResponse>();
        
        foreach (var mov in movements)
        {
            responses.Add(await MapToResponseAsync(mov));
        }
        
        return responses;
    }

    private async Task<StockMovementResponse> MapToResponseAsync(StockMovement movement)
    {
        var product = await _productRepository.GetByIdAsync(movement.ProductId);
        
        var location = await _storageLocationRepository.GetByIdAsync(movement.StorageLocationId);
        var warehouse = location != null ? await _warehouseRepository.GetByIdAsync(location.WarehouseId) : null;

        return new StockMovementResponse
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            ProductName = product?.Name ?? "N/A",
            WarehouseId = location?.WarehouseId ?? Guid.Empty,
            WarehouseName = warehouse?.Name ?? "N/A",
            StorageLocationId = movement.StorageLocationId,
            StorageLocationCode = location?.Code,
            Type = movement.Type,
            Quantity = movement.Quantity,
            Reference = movement.Reference,
            Notes = movement.Notes,
            MovementDate = movement.MovementDate
        };
    }
}
