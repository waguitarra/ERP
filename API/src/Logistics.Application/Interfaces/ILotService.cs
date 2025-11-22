using Logistics.Application.DTOs.Lot;

namespace Logistics.Application.Interfaces;

public interface ILotService
{
    Task<LotResponse> CreateAsync(CreateLotRequest request);
    Task<LotResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<LotResponse>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<LotResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<LotResponse>> GetExpiringLotsAsync(Guid companyId, int daysAhead);
    Task QuarantineLotAsync(Guid id);
    Task ReleaseLotAsync(Guid id);
}
