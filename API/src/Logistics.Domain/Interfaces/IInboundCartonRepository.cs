using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IInboundCartonRepository : IBaseRepository<InboundCarton>
{
    Task<InboundCarton?> GetByBarcodeAsync(string barcode);
    Task<IEnumerable<InboundCarton>> GetByParcelIdAsync(Guid parcelId);
}
