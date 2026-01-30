using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IPickingLineRepository
{
    Task<PickingLine?> GetByIdAsync(Guid id);
    Task<PickingLine?> GetByIdWithTaskAsync(Guid id, Guid taskId);
    Task<IEnumerable<PickingLine>> GetByTaskIdAsync(Guid taskId);
    Task UpdateAsync(PickingLine line);
}
