using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface ILotRepository
{
    Task<Lot?> GetByIdAsync(Guid id);
    Task<Lot?> GetByLotNumberAsync(string lotNumber, Guid companyId);
    Task<IEnumerable<Lot>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<Lot>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<Lot>> GetExpiringLotsAsync(Guid companyId, DateTime beforeDate);
    Task<IEnumerable<Lot>> GetAllAsync();
    Task AddAsync(Lot lot);
    Task UpdateAsync(Lot lot);
}
