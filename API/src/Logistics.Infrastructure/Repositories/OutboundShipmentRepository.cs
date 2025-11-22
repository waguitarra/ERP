using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class OutboundShipmentRepository : IOutboundShipmentRepository
{
    private readonly LogisticsDbContext _context;

    public OutboundShipmentRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<OutboundShipment?> GetByIdAsync(Guid id)
    {
        return await _context.OutboundShipments
            .Include(s => s.Order)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<OutboundShipment?> GetByShipmentNumberAsync(string shipmentNumber)
    {
        return await _context.OutboundShipments
            .FirstOrDefaultAsync(s => s.ShipmentNumber == shipmentNumber);
    }

    public async Task<IEnumerable<OutboundShipment>> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.OutboundShipments
            .Where(s => s.OrderId == orderId)
            .ToListAsync();
    }

    public async Task AddAsync(OutboundShipment shipment)
    {
        await _context.OutboundShipments.AddAsync(shipment);
    }

    public async Task UpdateAsync(OutboundShipment shipment)
    {
        _context.OutboundShipments.Update(shipment);
    }
}
