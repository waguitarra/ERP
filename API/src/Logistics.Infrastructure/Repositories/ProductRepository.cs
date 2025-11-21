using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(LogisticsDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Products
            .Where(p => p.CompanyId == companyId && p.IsActive)
            .ToListAsync();
    }

    public async Task<bool> SKUExistsAsync(string sku, Guid? excludeId = null)
    {
        var query = _context.Products.Where(p => p.SKU == sku);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
        
        return await query.AnyAsync();
    }

    public async Task<Product?> GetBySKUAsync(string sku)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Barcode == barcode);
    }
}
