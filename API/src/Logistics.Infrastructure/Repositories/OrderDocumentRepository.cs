using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class OrderDocumentRepository : BaseRepository<OrderDocument>, IOrderDocumentRepository
{
    public OrderDocumentRepository(LogisticsDbContext context) : base(context) { }

    public async Task<IEnumerable<OrderDocument>> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.OrderDocuments
            .Where(d => d.OrderId == orderId && d.DeletedAt == null)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }
}
