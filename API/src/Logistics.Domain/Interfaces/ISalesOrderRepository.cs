using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface ISalesOrderRepository
{
    Task<SalesOrder?> GetByIdAsync(Guid id);
    Task<SalesOrder?> GetBySalesOrderNumberAsync(string salesOrderNumber, Guid companyId);
    Task<IEnumerable<SalesOrder>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<SalesOrder>> GetByCustomerIdAsync(Guid customerId);
    Task AddAsync(SalesOrder salesOrder);
    Task UpdateAsync(SalesOrder salesOrder);
    Task DeleteAsync(Guid id);
}
