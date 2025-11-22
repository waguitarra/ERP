using Logistics.Application.DTOs.StockMovement;

namespace Logistics.Application.Interfaces;

public interface IStockMovementService
{
    Task<StockMovementResponse> CreateAsync(StockMovementRequest request);
    Task<StockMovementResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<StockMovementResponse>> GetAllAsync();
    Task<IEnumerable<StockMovementResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<StockMovementResponse>> GetByProductIdAsync(Guid productId);
}
