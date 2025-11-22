using Logistics.Application.DTOs.Inventory;

namespace Logistics.Application.Interfaces;

public interface IInventoryService
{
    Task<InventoryResponse> CreateAsync(InventoryRequest request);
    Task<InventoryResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<InventoryResponse>> GetAllAsync();
    Task<IEnumerable<InventoryResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<InventoryResponse>> GetByProductIdAsync(Guid productId);
    Task<InventoryResponse> UpdateAsync(Guid id, InventoryRequest request);
    Task DeleteAsync(Guid id);
}
