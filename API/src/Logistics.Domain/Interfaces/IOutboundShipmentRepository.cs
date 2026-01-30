using Logistics.Domain.Entities;
using Logistics.Domain.Enums;

namespace Logistics.Domain.Interfaces;

public interface IOutboundShipmentRepository
{
    Task<OutboundShipment?> GetByIdAsync(Guid id);
    Task<OutboundShipment?> GetByShipmentNumberAsync(string shipmentNumber);
    Task<IEnumerable<OutboundShipment>> GetAllAsync();
    Task<IEnumerable<OutboundShipment>> GetAllWithDetailsAsync();
    Task<IEnumerable<OutboundShipment>> GetByOrderIdAsync(Guid orderId);
    Task<IEnumerable<OutboundShipment>> GetByStatusAsync(OutboundStatus status);
    Task<IEnumerable<OutboundShipment>> GetPendingAsync();
    Task<IEnumerable<OutboundShipment>> GetShippedAsync();
    Task<IEnumerable<OutboundShipment>> GetInTransitAsync();
    Task AddAsync(OutboundShipment shipment);
    Task UpdateAsync(OutboundShipment shipment);
    Task DeleteAsync(OutboundShipment shipment);
}
