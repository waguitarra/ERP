using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly LogisticsDbContext _context;

    public PackageRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<Package?> GetByIdAsync(Guid id)
    {
        return await _context.Packages
            .Include(p => p.PackingTask)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Package?> GetByTrackingNumberAsync(string trackingNumber)
    {
        return await _context.Packages
            .FirstOrDefaultAsync(p => p.TrackingNumber == trackingNumber);
    }

    public async Task<IEnumerable<Package>> GetByPackingTaskIdAsync(Guid packingTaskId)
    {
        return await _context.Packages
            .Where(p => p.PackingTaskId == packingTaskId)
            .ToListAsync();
    }

    public async Task AddAsync(Package package)
    {
        await _context.Packages.AddAsync(package);
    }

    public async Task UpdateAsync(Package package)
    {
        _context.Packages.Update(package);
    }
}
