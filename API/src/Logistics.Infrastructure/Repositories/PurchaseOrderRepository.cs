using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly LogisticsDbContext _context;

    public PurchaseOrderRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Include(po => po.Documents)
            .Include(po => po.Supplier)
            .FirstOrDefaultAsync(po => po.Id == id);
    }

    public async Task AddAsync(PurchaseOrder purchaseOrder)
    {
        await _context.PurchaseOrders.AddAsync(purchaseOrder);
    }

    public async Task UpdateAsync(PurchaseOrder purchaseOrder)
    {
        _context.PurchaseOrders.Update(purchaseOrder);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.PurchaseOrders.Remove(entity);
        }
    }

    public async Task<PurchaseOrder?> GetByPurchaseOrderNumberAsync(string purchaseOrderNumber, Guid companyId)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Include(po => po.Documents)
            .FirstOrDefaultAsync(po => po.PurchaseOrderNumber == purchaseOrderNumber && po.CompanyId == companyId);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Include(po => po.Supplier)
            .Where(po => po.CompanyId == companyId)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(Guid supplierId)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .Where(po => po.SupplierId == supplierId)
            .OrderByDescending(po => po.OrderDate)
            .ToListAsync();
    }
}
