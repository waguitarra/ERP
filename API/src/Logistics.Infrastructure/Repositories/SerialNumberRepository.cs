using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class SerialNumberRepository : ISerialNumberRepository
{
    private readonly LogisticsDbContext _context;

    public SerialNumberRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<SerialNumber?> GetByIdAsync(Guid id)
    {
        return await _context.SerialNumbers
            .Include(s => s.Product)
            .Include(s => s.Lot)
            .Include(s => s.CurrentLocation)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<SerialNumber?> GetBySerialAsync(string serial)
    {
        return await _context.SerialNumbers
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Serial == serial);
    }

    public async Task<IEnumerable<SerialNumber>> GetByProductIdAsync(Guid productId)
    {
        return await _context.SerialNumbers
            .Where(s => s.ProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SerialNumber>> GetByLotIdAsync(Guid lotId)
    {
        return await _context.SerialNumbers
            .Where(s => s.LotId == lotId)
            .ToListAsync();
    }

    public async Task AddAsync(SerialNumber serialNumber)
    {
        await _context.SerialNumbers.AddAsync(serialNumber);
    }

    public async Task UpdateAsync(SerialNumber serialNumber)
    {
        _context.SerialNumbers.Update(serialNumber);
    }
}
