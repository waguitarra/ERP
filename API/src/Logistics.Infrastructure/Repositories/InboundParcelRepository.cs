using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class InboundParcelRepository : BaseRepository<InboundParcel>, IInboundParcelRepository
{
    public InboundParcelRepository(LogisticsDbContext context) : base(context) { }

    public async Task<InboundParcel?> GetByLPNAsync(string lpn)
    {
        return await _context.InboundParcels
            .Include(p => p.Items)
            .Include(p => p.Cartons)
            .FirstOrDefaultAsync(p => p.LPN == lpn);
    }

    public async Task<IEnumerable<InboundParcel>> GetByShipmentIdAsync(Guid shipmentId)
    {
        return await _context.InboundParcels
            .Include(p => p.Items)
            .Include(p => p.Cartons)
            .Where(p => p.InboundShipmentId == shipmentId)
            .OrderBy(p => p.SequenceNumber)
            .ToListAsync();
    }
}
