using Logistics.Application.DTOs.Driver;

namespace Logistics.Application.Interfaces;

public interface IDriverService
{
    Task<DriverResponse> CreateAsync(DriverRequest request);
    Task<DriverResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<DriverResponse>> GetAllAsync();
    Task<IEnumerable<DriverResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<DriverResponse> UpdateAsync(Guid id, DriverRequest request);
    Task DeleteAsync(Guid id);
    Task ActivateAsync(Guid id);
    Task DeactivateAsync(Guid id);
}
