using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class ReceiptRepository : IReceiptRepository
{
    private readonly LogisticsDbContext _context;

    public ReceiptRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<Receipt?> GetByIdAsync(Guid id)
    {
        return await _context.Receipts
            .Include(r => r.Lines)
            .ThenInclude(l => l.Product)
            .Include(r => r.InboundShipment)
            .Include(r => r.Warehouse)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Receipt?> GetByReceiptNumberAsync(string receiptNumber)
    {
        return await _context.Receipts
            .Include(r => r.Lines)
            .FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber);
    }

    public async Task<IEnumerable<Receipt>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _context.Receipts
            .Include(r => r.Lines)
            .Where(r => r.WarehouseId == warehouseId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Receipt>> GetAllAsync()
    {
        return await _context.Receipts
            .Include(r => r.Lines)
            .ToListAsync();
    }

    public async Task AddAsync(Receipt receipt)
    {
        await _context.Receipts.AddAsync(receipt);
    }

    public Task UpdateAsync(Receipt receipt)
    {
        _context.Receipts.Update(receipt);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var receipt = await GetByIdAsync(id);
        if (receipt != null)
            _context.Receipts.Remove(receipt);
    }
}
