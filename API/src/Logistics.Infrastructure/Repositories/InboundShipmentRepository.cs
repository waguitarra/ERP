using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class InboundShipmentRepository : IInboundShipmentRepository
{
    private readonly LogisticsDbContext _context;

    public InboundShipmentRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<InboundShipment?> GetByIdAsync(Guid id)
    {
        return await _context.InboundShipments
            .Include(i => i.Supplier)
            .Include(i => i.Order)
                .ThenInclude(o => o.Items)
            .Include(i => i.Vehicle)
            .Include(i => i.Driver)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<InboundShipment?> GetByShipmentNumberAsync(string shipmentNumber, Guid companyId)
    {
        return await _context.InboundShipments
            .FirstOrDefaultAsync(i => i.ShipmentNumber == shipmentNumber && i.CompanyId == companyId);
    }

    public async Task<IEnumerable<InboundShipment>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.InboundShipments
            .Include(i => i.Supplier)
            .Where(i => i.CompanyId == companyId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<InboundShipment>> GetByWarehouseAsync(Guid warehouseId)
    {
        return await _context.InboundShipments
            .Where(i => i.Order.CompanyId == i.CompanyId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<InboundShipment>> GetAllAsync()
    {
        return await _context.InboundShipments
            .Include(i => i.Supplier)
            .ToListAsync();
    }

    public async Task AddAsync(InboundShipment shipment)
    {
        await _context.InboundShipments.AddAsync(shipment);
    }

    public Task UpdateAsync(InboundShipment shipment)
    {
        _context.InboundShipments.Update(shipment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var shipment = await GetByIdAsync(id);
        if (shipment != null)
            _context.InboundShipments.Remove(shipment);
    }
}
