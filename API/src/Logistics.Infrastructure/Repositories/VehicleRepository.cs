using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(LogisticsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Vehicle>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet
            .Where(v => v.CompanyId == companyId)
            .OrderBy(v => v.LicensePlate)
            .ToListAsync();
    }

    public async Task<bool> LicensePlateExistsAsync(string licensePlate, Guid? excludeId = null)
    {
        var query = _dbSet.Where(v => v.LicensePlate == licensePlate);
        
        if (excludeId.HasValue)
        {
            query = query.Where(v => v.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public override async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(v => v.Company)
            .FirstOrDefaultAsync(v => v.Id == id);
    }
}
