using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
{
    public CompanyRepository(LogisticsDbContext context) : base(context)
    {
    }

    public async Task<Company?> GetByDocumentAsync(string document)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Document == document);
    }

    public async Task<bool> DocumentExistsAsync(string document, Guid? excludeId = null)
    {
        var query = _dbSet.Where(c => c.Document == document);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public override async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
