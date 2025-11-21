using Logistics.Application.DTOs.Vehicle;

namespace Logistics.Application.Interfaces;

public interface IVehicleService
{
    Task<VehicleResponse> CreateAsync(VehicleRequest request);
    Task<VehicleResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<VehicleResponse>> GetAllAsync();
    Task<IEnumerable<VehicleResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<VehicleResponse> UpdateAsync(Guid id, VehicleRequest request);
    Task DeleteAsync(Guid id);
    Task<VehicleResponse> UpdateStatusAsync(Guid id, string status);
}
