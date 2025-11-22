using Logistics.Application.DTOs.StorageLocation;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class StorageLocationService : IStorageLocationService
{
    private readonly IStorageLocationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public StorageLocationService(IStorageLocationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StorageLocationResponse> CreateAsync(StorageLocationRequest request)
    {
        var storageLocation = new StorageLocation(
            request.WarehouseId,
            null, // ZoneId
            request.Code,
            request.Description
        );

        await _repository.AddAsync(storageLocation);
        await _unitOfWork.CommitAsync();

        return MapToResponse(storageLocation);
    }

    public async Task<StorageLocationResponse> GetByIdAsync(Guid id)
    {
        var storageLocation = await _repository.GetByIdAsync(id);
        if (storageLocation == null)
            throw new KeyNotFoundException($"Localização de armazenamento não encontrada: {id}");

        return MapToResponse(storageLocation);
    }

    public async Task<IEnumerable<StorageLocationResponse>> GetAllAsync()
    {
        var storageLocations = await _repository.GetAllAsync();
        return storageLocations.Select(MapToResponse);
    }

    public async Task<IEnumerable<StorageLocationResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var storageLocations = await _repository.GetByWarehouseIdAsync(warehouseId);
        return storageLocations.Select(MapToResponse);
    }

    public async Task<StorageLocationResponse> UpdateAsync(Guid id, StorageLocationRequest request)
    {
        var storageLocation = await _repository.GetByIdAsync(id);
        if (storageLocation == null)
            throw new KeyNotFoundException($"Localização de armazenamento não encontrada: {id}");

        storageLocation.Update(request.Code, request.Description);
        await _unitOfWork.CommitAsync();

        return MapToResponse(storageLocation);
    }

    public async Task DeleteAsync(Guid id)
    {
        var storageLocation = await _repository.GetByIdAsync(id);
        if (storageLocation == null)
            throw new KeyNotFoundException($"Localização de armazenamento não encontrada: {id}");

        storageLocation.Deactivate();
        await _unitOfWork.CommitAsync();
    }

    private static StorageLocationResponse MapToResponse(StorageLocation storageLocation)
    {
        return new StorageLocationResponse
        {
            Id = storageLocation.Id,
            WarehouseId = storageLocation.WarehouseId,
            Code = storageLocation.Code,
            Description = storageLocation.Description,
            Capacity = 0,
            IsActive = storageLocation.IsActive,
            CreatedAt = storageLocation.CreatedAt
        };
    }
}
