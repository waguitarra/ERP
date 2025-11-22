using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface ISerialNumberRepository
{
    Task<SerialNumber?> GetByIdAsync(Guid id);
    Task<SerialNumber?> GetBySerialAsync(string serial);
    Task<IEnumerable<SerialNumber>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<SerialNumber>> GetByLotIdAsync(Guid lotId);
    Task AddAsync(SerialNumber serialNumber);
    Task UpdateAsync(SerialNumber serialNumber);
}
