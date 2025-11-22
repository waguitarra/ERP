using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IReceiptRepository
{
    Task<Receipt?> GetByIdAsync(Guid id);
    Task<Receipt?> GetByReceiptNumberAsync(string receiptNumber);
    Task<IEnumerable<Receipt>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<Receipt>> GetAllAsync();
    Task AddAsync(Receipt receipt);
    Task UpdateAsync(Receipt receipt);
    Task DeleteAsync(Guid id);
}
