using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class LotRepository : ILotRepository
{
    private readonly LogisticsDbContext _context;

    public LotRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<Lot?> GetByIdAsync(Guid id)
    {
        return await _context.Lots
            .Include(l => l.Product)
            .Include(l => l.Supplier)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Lot?> GetByLotNumberAsync(string lotNumber, Guid companyId)
    {
        return await _context.Lots
            .Include(l => l.Product)
            .FirstOrDefaultAsync(l => l.LotNumber == lotNumber && l.CompanyId == companyId);
    }

    public async Task<IEnumerable<Lot>> GetByProductIdAsync(Guid productId)
    {
        return await _context.Lots
            .Include(l => l.Product)
            .Where(l => l.ProductId == productId)
            .OrderBy(l => l.ExpiryDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lot>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Lots
            .Include(l => l.Product)
            .Where(l => l.CompanyId == companyId)
            .OrderBy(l => l.ExpiryDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lot>> GetExpiringLotsAsync(Guid companyId, DateTime beforeDate)
    {
        return await _context.Lots
            .Include(l => l.Product)
            .Where(l => l.CompanyId == companyId && l.ExpiryDate <= beforeDate && l.QuantityAvailable > 0)
            .OrderBy(l => l.ExpiryDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lot>> GetAllAsync()
    {
        return await _context.Lots
            .Include(l => l.Product)
            .OrderBy(l => l.ExpiryDate)
            .ToListAsync();
    }

    public async Task AddAsync(Lot lot)
    {
        await _context.Lots.AddAsync(lot);
    }

    public async Task UpdateAsync(Lot lot)
    {
        _context.Lots.Update(lot);
    }
}
