using Logistics.Application.DTOs.OutboundShipment;
using Logistics.Domain.Enums;

namespace Logistics.Application.Interfaces;

public interface IOutboundShipmentService
{
    Task<OutboundShipmentResponse> CreateAsync(CreateOutboundShipmentRequest request);
    Task<OutboundShipmentResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<OutboundShipmentResponse>> GetAllAsync();
    Task<IEnumerable<OutboundShipmentResponse>> GetByOrderIdAsync(Guid orderId);
    Task<IEnumerable<OutboundShipmentResponse>> GetByStatusAsync(OutboundStatus status);
    Task<IEnumerable<OutboundShipmentResponse>> GetPendingAsync();
    Task<IEnumerable<OutboundShipmentResponse>> GetShippedAsync();
    Task<IEnumerable<OutboundShipmentResponse>> GetInTransitAsync();
    Task MarkReadyToShipAsync(Guid id);
    Task ShipAsync(Guid id);
    Task MarkInTransitAsync(Guid id);
    Task DeliverAsync(Guid id);
    Task CancelAsync(Guid id);
    Task DeleteAsync(Guid id);
}
