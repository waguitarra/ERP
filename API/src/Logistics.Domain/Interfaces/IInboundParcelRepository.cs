using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IInboundParcelRepository : IBaseRepository<InboundParcel>
{
    Task<InboundParcel?> GetByLPNAsync(string lpn);
    Task<IEnumerable<InboundParcel>> GetByShipmentIdAsync(Guid shipmentId);
}
