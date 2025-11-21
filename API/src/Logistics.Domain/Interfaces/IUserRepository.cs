using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<IEnumerable<User>> GetByCompanyIdAsync(Guid companyId);
}
