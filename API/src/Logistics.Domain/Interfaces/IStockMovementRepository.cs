using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IStockMovementRepository : IBaseRepository<StockMovement>
{
    Task<IEnumerable<StockMovement>> GetByProductIdAsync(Guid productId);
}
