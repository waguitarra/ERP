using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class SalesOrderRepository : ISalesOrderRepository
{
    private readonly LogisticsDbContext _context;

    public SalesOrderRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<SalesOrder?> GetByIdAsync(Guid id)
    {
        return await _context.SalesOrders
            .Include(so => so.Items)
            .Include(so => so.Customer)
            .FirstOrDefaultAsync(so => so.Id == id);
    }

    public async Task AddAsync(SalesOrder salesOrder)
    {
        await _context.SalesOrders.AddAsync(salesOrder);
    }

    public async Task UpdateAsync(SalesOrder salesOrder)
    {
        _context.SalesOrders.Update(salesOrder);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.SalesOrders.Remove(entity);
        }
    }

    public async Task<SalesOrder?> GetBySalesOrderNumberAsync(string salesOrderNumber, Guid companyId)
    {
        return await _context.SalesOrders
            .Include(so => so.Items)
            .FirstOrDefaultAsync(so => so.SalesOrderNumber == salesOrderNumber && so.CompanyId == companyId);
    }

    public async Task<IEnumerable<SalesOrder>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.SalesOrders
            .Include(so => so.Items)
            .Include(so => so.Customer)
            .Where(so => so.CompanyId == companyId)
            .OrderByDescending(so => so.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<SalesOrder>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.SalesOrders
            .Include(so => so.Items)
            .Where(so => so.CustomerId == customerId)
            .OrderByDescending(so => so.OrderDate)
            .ToListAsync();
    }
}
