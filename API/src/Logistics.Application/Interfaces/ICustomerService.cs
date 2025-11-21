using Logistics.Application.DTOs.Customer;
namespace Logistics.Application.Interfaces;
public interface ICustomerService
{
    Task<CustomerResponse> CreateAsync(CustomerRequest request);
    Task<CustomerResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<CustomerResponse>> GetAllAsync();
    Task<IEnumerable<CustomerResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<CustomerResponse> UpdateAsync(Guid id, CustomerRequest request);
    Task DeleteAsync(Guid id);
}
