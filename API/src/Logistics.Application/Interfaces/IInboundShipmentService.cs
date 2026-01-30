using Logistics.Application.DTOs.InboundShipment;
using Logistics.Domain.Enums;

namespace Logistics.Application.Interfaces;

public interface IInboundShipmentService
{
    Task<InboundShipmentResponse> CreateAsync(CreateInboundShipmentRequest request);
    Task<InboundShipmentResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<InboundShipmentResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<InboundShipmentResponse>> GetAllAsync();
    Task<IEnumerable<InboundShipmentResponse>> GetByStatusAsync(InboundStatus status);
    Task<IEnumerable<InboundShipmentResponse>> GetScheduledAsync();
    Task<IEnumerable<InboundShipmentResponse>> GetInProgressAsync();
    Task ReceiveAsync(Guid id, Guid receivedBy);
    Task CompleteAsync(Guid id);
    Task CancelAsync(Guid id);
    Task DeleteAsync(Guid id);
}
