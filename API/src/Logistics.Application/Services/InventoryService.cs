using Logistics.Application.DTOs.Inventory;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(
        IInventoryRepository repository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IStorageLocationRepository storageLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _storageLocationRepository = storageLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InventoryResponse> CreateAsync(InventoryRequest request)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
            throw new KeyNotFoundException($"Produto não encontrado: {request.ProductId}");

        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId);
        if (warehouse == null)
            throw new KeyNotFoundException($"Armazém não encontrado: {request.WarehouseId}");

        var inventory = new Inventory(
            request.ProductId,
            request.StorageLocationId ?? Guid.Empty,
            request.Quantity
        );

        await _repository.AddAsync(inventory);
        await _unitOfWork.CommitAsync();

        return await MapToResponseAsync(inventory);
    }

    public async Task<InventoryResponse> GetByIdAsync(Guid id)
    {
        var inventory = await _repository.GetByIdAsync(id);
        if (inventory == null)
            throw new KeyNotFoundException($"Inventário não encontrado: {id}");

        return await MapToResponseAsync(inventory);
    }

    public async Task<IEnumerable<InventoryResponse>> GetAllAsync()
    {
        var inventories = await _repository.GetAllAsync();
        var responses = new List<InventoryResponse>();
        
        foreach (var inv in inventories)
        {
            responses.Add(await MapToResponseAsync(inv));
        }
        
        return responses;
    }

    public async Task<IEnumerable<InventoryResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var allInventories = await _repository.GetAllAsync();
        var inventories = allInventories.Where(i => 
        {
            var loc = _storageLocationRepository.GetByIdAsync(i.StorageLocationId).Result;
            return loc?.WarehouseId == warehouseId;
        });
        
        var responses = new List<InventoryResponse>();
        foreach (var inv in inventories)
        {
            responses.Add(await MapToResponseAsync(inv));
        }
        
        return responses;
    }

    public async Task<IEnumerable<InventoryResponse>> GetByProductIdAsync(Guid productId)
    {
        var inventories = await _repository.GetByProductIdAsync(productId);
        var responses = new List<InventoryResponse>();
        
        foreach (var inv in inventories)
        {
            responses.Add(await MapToResponseAsync(inv));
        }
        
        return responses;
    }

    public async Task<InventoryResponse> UpdateAsync(Guid id, InventoryRequest request)
    {
        var inventory = await _repository.GetByIdAsync(id);
        if (inventory == null)
            throw new KeyNotFoundException($"Inventário não encontrado: {id}");

        if (request.Quantity > inventory.Quantity)
            inventory.AddStock(request.Quantity - inventory.Quantity);
        else if (request.Quantity < inventory.Quantity)
            inventory.RemoveStock(inventory.Quantity - request.Quantity);
        await _unitOfWork.CommitAsync();

        return await MapToResponseAsync(inventory);
    }

    public async Task DeleteAsync(Guid id)
    {
        var inventory = await _repository.GetByIdAsync(id);
        if (inventory == null)
            throw new KeyNotFoundException($"Inventário não encontrado: {id}");

        // Zerar quantidade e atualizar
        inventory.RemoveStock(inventory.Quantity);
        await _unitOfWork.CommitAsync();
    }

    private async Task<InventoryResponse> MapToResponseAsync(Inventory inventory)
    {
        var product = await _productRepository.GetByIdAsync(inventory.ProductId);
        var location = await _storageLocationRepository.GetByIdAsync(inventory.StorageLocationId);
        var warehouse = location != null ? await _warehouseRepository.GetByIdAsync(location.WarehouseId) : null;

        return new InventoryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductName = product?.Name ?? "N/A",
            WarehouseId = location?.WarehouseId ?? Guid.Empty,
            WarehouseName = warehouse?.Name ?? "N/A",
            StorageLocationId = inventory.StorageLocationId,
            StorageLocationCode = location?.Code,
            Quantity = (int)inventory.Quantity,
            MinimumStock = 0,
            MaximumStock = 0,
            LastUpdated = inventory.LastUpdatedAt
        };
    }
}
