using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class InboundCartonRepository : BaseRepository<InboundCarton>, IInboundCartonRepository
{
    public InboundCartonRepository(LogisticsDbContext context) : base(context) { }

    public async Task<InboundCarton?> GetByBarcodeAsync(string barcode)
    {
        return await _context.InboundCartons
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Barcode == barcode);
    }

    public async Task<IEnumerable<InboundCarton>> GetByParcelIdAsync(Guid parcelId)
    {
        return await _context.InboundCartons
            .Include(c => c.Items)
            .Where(c => c.InboundParcelId == parcelId)
            .OrderBy(c => c.SequenceNumber)
            .ToListAsync();
    }
}
