using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class DriverRepository : BaseRepository<Driver>, IDriverRepository
{
    public DriverRepository(LogisticsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Driver>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet
            .Where(d => d.CompanyId == companyId && d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<bool> LicenseNumberExistsAsync(string licenseNumber, Guid? excludeId = null)
    {
        var query = _dbSet.Where(d => d.LicenseNumber == licenseNumber);
        
        if (excludeId.HasValue)
        {
            query = query.Where(d => d.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public override async Task<Driver?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(d => d.Company)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}
