using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetByDocumentAsync(string document);
    Task<bool> DocumentExistsAsync(string document, Guid? excludeId = null);
}
