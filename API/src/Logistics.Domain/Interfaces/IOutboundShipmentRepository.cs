using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IOutboundShipmentRepository
{
    Task<OutboundShipment?> GetByIdAsync(Guid id);
    Task<OutboundShipment?> GetByShipmentNumberAsync(string shipmentNumber);
    Task<IEnumerable<OutboundShipment>> GetByOrderIdAsync(Guid orderId);
    Task AddAsync(OutboundShipment shipment);
    Task UpdateAsync(OutboundShipment shipment);
}
