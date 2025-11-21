using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(LogisticsDbContext context) : base(context) { }

    public async Task<IEnumerable<Supplier>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Suppliers.Where(s => s.CompanyId == companyId && s.IsActive).ToListAsync();
    }

    public async Task<bool> DocumentExistsAsync(string document, Guid? excludeId = null)
    {
        var query = _context.Suppliers.Where(s => s.Document == document);
        if (excludeId.HasValue) query = query.Where(s => s.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
