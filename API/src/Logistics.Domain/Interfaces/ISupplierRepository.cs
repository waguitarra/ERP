using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface ISupplierRepository : IBaseRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetByCompanyIdAsync(Guid companyId);
    Task<bool> DocumentExistsAsync(string document, Guid? excludeId = null);
}
