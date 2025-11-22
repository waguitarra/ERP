using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, Guid companyId);
    Task<IEnumerable<Order>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<Order>> GetAllAsync();
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Guid id);
}
