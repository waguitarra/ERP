using Logistics.Application.DTOs.Company;

namespace Logistics.Application.Interfaces;

public interface ICompanyService
{
    Task<CompanyResponse> CreateAsync(CompanyRequest request);
    Task<CompanyResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<CompanyResponse>> GetAllAsync();
    Task<CompanyResponse> UpdateAsync(Guid id, CompanyRequest request);
    Task DeleteAsync(Guid id);
}
