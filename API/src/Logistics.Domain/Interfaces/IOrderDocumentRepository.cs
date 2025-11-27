using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IOrderDocumentRepository : IBaseRepository<OrderDocument>
{
    Task<IEnumerable<OrderDocument>> GetByOrderIdAsync(Guid orderId);
}
