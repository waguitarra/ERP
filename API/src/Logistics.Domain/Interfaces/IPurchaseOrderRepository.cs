using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(Guid id);
    Task<PurchaseOrder?> GetByPurchaseOrderNumberAsync(string purchaseOrderNumber, Guid companyId);
    Task<IEnumerable<PurchaseOrder>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(Guid supplierId);
    Task AddAsync(PurchaseOrder purchaseOrder);
    Task UpdateAsync(PurchaseOrder purchaseOrder);
    Task DeleteAsync(Guid id);
}
