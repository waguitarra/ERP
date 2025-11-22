using Logistics.Application.DTOs.SerialNumber;

namespace Logistics.Application.Interfaces;

public interface ISerialNumberService
{
    Task<SerialNumberResponse> CreateAsync(CreateSerialNumberRequest request);
    Task<SerialNumberResponse> GetByIdAsync(Guid id);
    Task<SerialNumberResponse> GetBySerialAsync(string serial);
    Task<IEnumerable<SerialNumberResponse>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<SerialNumberResponse>> GetByLotIdAsync(Guid lotId);
    Task ReceiveAsync(Guid id, Guid locationId);
    Task ShipAsync(Guid id);
}
