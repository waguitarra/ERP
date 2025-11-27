using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly LogisticsDbContext _context;

    public ProductCategoryRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<ProductCategory?> GetByIdAsync(Guid id)
    {
        return await _context.ProductCategories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ProductCategory?> GetByCodeAsync(string code)
    {
        return await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<IEnumerable<ProductCategory>> GetAllAsync()
    {
        return await _context.ProductCategories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductCategory>> GetActiveAsync()
    {
        return await _context.ProductCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task AddAsync(ProductCategory category)
    {
        await _context.ProductCategories.AddAsync(category);
    }

    public async Task UpdateAsync(ProductCategory category)
    {
        _context.ProductCategories.Update(category);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category != null)
        {
            _context.ProductCategories.Remove(category);
        }
    }
}
