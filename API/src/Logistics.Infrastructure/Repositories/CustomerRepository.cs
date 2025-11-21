using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(LogisticsDbContext context) : base(context) { }

    public async Task<IEnumerable<Customer>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Customers.Where(c => c.CompanyId == companyId && c.IsActive).ToListAsync();
    }

    public async Task<bool> DocumentExistsAsync(string document, Guid? excludeId = null)
    {
        var query = _context.Customers.Where(c => c.Document == document);
        if (excludeId.HasValue) query = query.Where(c => c.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
