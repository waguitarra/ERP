using Logistics.Application.DTOs.Product;

namespace Logistics.Application.Interfaces;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(ProductRequest request);
    Task<ProductResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductResponse>> GetAllAsync();
    Task<IEnumerable<ProductResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<ProductResponse> UpdateAsync(Guid id, ProductRequest request);
    Task DeleteAsync(Guid id);
}
