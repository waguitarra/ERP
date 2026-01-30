using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
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

    public async Task<IEnumerable<OutboundShipment>> GetAllAsync()
    {
        return await _context.OutboundShipments
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OutboundShipment>> GetAllWithDetailsAsync()
    {
        return await _context.OutboundShipments
            .Include(s => s.Order)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OutboundShipment>> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.OutboundShipments
            .Where(s => s.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<OutboundShipment>> GetByStatusAsync(OutboundStatus status)
    {
        return await _context.OutboundShipments
            .Include(s => s.Order)
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OutboundShipment>> GetPendingAsync()
    {
        return await _context.OutboundShipments
            .Include(s => s.Order)
            .Where(s => s.Status == OutboundStatus.Pending)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<OutboundShipment>> GetShippedAsync()
    {
        return await _context.OutboundShipments
            .Include(s => s.Order)
            .Where(s => s.Status == OutboundStatus.Shipped)
            .OrderByDescending(s => s.ShippedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<OutboundShipment>> GetInTransitAsync()
    {
        return await _context.OutboundShipments
            .Include(s => s.Order)
            .Where(s => s.Status == OutboundStatus.InTransit)
            .OrderByDescending(s => s.ShippedDate)
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

    public async Task DeleteAsync(OutboundShipment shipment)
    {
        _context.OutboundShipments.Remove(shipment);
        await Task.CompletedTask;
    }
}
