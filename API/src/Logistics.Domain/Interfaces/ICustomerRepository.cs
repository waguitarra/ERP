using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<IEnumerable<Customer>> GetByCompanyIdAsync(Guid companyId);
    Task<bool> DocumentExistsAsync(string document, Guid? excludeId = null);
}
