using Logistics.Application.DTOs.Supplier;
namespace Logistics.Application.Interfaces;
public interface ISupplierService
{
    Task<SupplierResponse> CreateAsync(SupplierRequest request);
    Task<SupplierResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<SupplierResponse>> GetAllAsync();
    Task<IEnumerable<SupplierResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<SupplierResponse> UpdateAsync(Guid id, SupplierRequest request);
    Task DeleteAsync(Guid id);
}
