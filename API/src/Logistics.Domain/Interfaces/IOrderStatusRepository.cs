using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IOrderStatusRepository : IBaseRepository<OrderStatusConfig>
{
    Task<OrderStatusConfig?> GetByCodeAsync(string code);
    Task<IEnumerable<OrderStatusConfig>> GetAllActiveAsync();
}
