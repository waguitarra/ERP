using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IDriverRepository : IBaseRepository<Driver>
{
    Task<IEnumerable<Driver>> GetByCompanyIdAsync(Guid companyId);
    Task<bool> LicenseNumberExistsAsync(string licenseNumber, Guid? excludeId = null);
}
