using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IWarehouseRepository : IBaseRepository<Warehouse>
{
    Task<IEnumerable<Warehouse>> GetByCompanyIdAsync(Guid companyId);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
}
