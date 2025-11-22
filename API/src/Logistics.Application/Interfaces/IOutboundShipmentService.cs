using Logistics.Application.DTOs.OutboundShipment;

namespace Logistics.Application.Interfaces;

public interface IOutboundShipmentService
{
    Task<OutboundShipmentResponse> CreateAsync(CreateOutboundShipmentRequest request);
    Task<OutboundShipmentResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<OutboundShipmentResponse>> GetByOrderIdAsync(Guid orderId);
    Task ShipAsync(Guid id);
    Task DeliverAsync(Guid id);
}
