using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(LogisticsDbContext context) : base(context) { }
    public async Task<IEnumerable<Warehouse>> GetByCompanyIdAsync(Guid companyId) =>
        await _context.Warehouses.Where(w => w.CompanyId == companyId && w.IsActive).ToListAsync();
    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
    {
        var query = _context.Warehouses.Where(w => w.Code == code);
        if (excludeId.HasValue) query = query.Where(w => w.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
