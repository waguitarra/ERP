using Logistics.Application.DTOs.StorageLocation;

namespace Logistics.Application.Interfaces;

public interface IStorageLocationService
{
    Task<StorageLocationResponse> CreateAsync(StorageLocationRequest request);
    Task<StorageLocationResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<StorageLocationResponse>> GetAllAsync();
    Task<IEnumerable<StorageLocationResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<StorageLocationResponse> UpdateAsync(Guid id, StorageLocationRequest request);
    Task DeleteAsync(Guid id);
}
