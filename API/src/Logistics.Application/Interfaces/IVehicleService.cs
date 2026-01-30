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
    
    // Tracking methods
    Task<VehicleResponse> EnableTrackingAsync(Guid id);
    Task<VehicleResponse> DisableTrackingAsync(Guid id);
    Task<VehicleResponse> RegenerateTrackingTokenAsync(Guid id);
    Task<VehicleResponse> UpdateLocationAsync(Guid id, UpdateVehicleLocationRequest request);
    Task<IEnumerable<VehicleResponse>> GetWithTrackingEnabledAsync();
    
    // Driver management
    Task<VehicleResponse> AssignDriverAsync(Guid id, AssignDriverRequest request);
    Task<VehicleResponse> RemoveDriverAsync(Guid id);
}
