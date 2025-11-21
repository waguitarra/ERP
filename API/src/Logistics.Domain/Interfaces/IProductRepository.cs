using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product>> GetByCompanyIdAsync(Guid companyId);
    Task<bool> SKUExistsAsync(string sku, Guid? excludeId = null);
    Task<Product?> GetBySKUAsync(string sku);
    Task<Product?> GetByBarcodeAsync(string barcode);
}
