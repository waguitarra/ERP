using Logistics.Application.DTOs.Package;

namespace Logistics.Application.Interfaces;

public interface IPackageService
{
    Task<PackageResponse> CreateAsync(CreatePackageRequest request);
    Task<PackageResponse> GetByIdAsync(Guid id);
    Task<PackageResponse> GetByTrackingNumberAsync(string trackingNumber);
    Task<IEnumerable<PackageResponse>> GetByPackingTaskIdAsync(Guid packingTaskId);
    Task ShipPackageAsync(Guid id);
}
