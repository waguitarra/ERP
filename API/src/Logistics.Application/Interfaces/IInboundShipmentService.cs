using Logistics.Application.DTOs.InboundShipment;

namespace Logistics.Application.Interfaces;

public interface IInboundShipmentService
{
    Task<InboundShipmentResponse> CreateAsync(CreateInboundShipmentRequest request);
    Task<InboundShipmentResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<InboundShipmentResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<InboundShipmentResponse>> GetAllAsync();
    Task ReceiveAsync(Guid id, Guid receivedBy);
    Task CompleteAsync(Guid id);
}
