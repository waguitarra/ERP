using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IProductCategoryRepository
{
    Task<ProductCategory?> GetByIdAsync(Guid id);
    Task<ProductCategory?> GetByCodeAsync(string code);
    Task<IEnumerable<ProductCategory>> GetAllAsync();
    Task<IEnumerable<ProductCategory>> GetActiveAsync();
    Task AddAsync(ProductCategory category);
    Task UpdateAsync(ProductCategory category);
    Task DeleteAsync(Guid id);
}
