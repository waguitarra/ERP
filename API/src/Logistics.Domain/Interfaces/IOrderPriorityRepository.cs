using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IOrderPriorityRepository : IBaseRepository<OrderPriorityConfig>
{
    Task<OrderPriorityConfig?> GetByCodeAsync(string code);
    Task<IEnumerable<OrderPriorityConfig>> GetAllActiveAsync();
}
