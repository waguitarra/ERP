using Logistics.Domain.Entities;
using Logistics.Domain.Enums;

namespace Logistics.Domain.Interfaces;

public interface IInboundShipmentRepository
{
    Task<InboundShipment?> GetByIdAsync(Guid id);
    Task<InboundShipment?> GetByShipmentNumberAsync(string shipmentNumber, Guid companyId);
    Task<IEnumerable<InboundShipment>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<InboundShipment>> GetByWarehouseAsync(Guid warehouseId);
    Task<IEnumerable<InboundShipment>> GetAllAsync();
    Task<IEnumerable<InboundShipment>> GetAllWithDetailsAsync();
    Task<IEnumerable<InboundShipment>> GetByStatusAsync(InboundStatus status);
    Task<IEnumerable<InboundShipment>> GetScheduledAsync();
    Task<IEnumerable<InboundShipment>> GetInProgressAsync();
    Task AddAsync(InboundShipment shipment);
    Task UpdateAsync(InboundShipment shipment);
    Task DeleteAsync(Guid id);
}
