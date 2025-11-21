using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IVehicleRepository : IBaseRepository<Vehicle>
{
    Task<IEnumerable<Vehicle>> GetByCompanyIdAsync(Guid companyId);
    Task<bool> LicensePlateExistsAsync(string licensePlate, Guid? excludeId = null);
}
