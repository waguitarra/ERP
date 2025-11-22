using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IPackageRepository
{
    Task<Package?> GetByIdAsync(Guid id);
    Task<Package?> GetByTrackingNumberAsync(string trackingNumber);
    Task<IEnumerable<Package>> GetByPackingTaskIdAsync(Guid packingTaskId);
    Task AddAsync(Package package);
    Task UpdateAsync(Package package);
}
