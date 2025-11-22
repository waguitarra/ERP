using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IPickingWaveRepository
{
    Task<PickingWave?> GetByIdAsync(Guid id);
    Task<PickingWave?> GetByWaveNumberAsync(string waveNumber);
    Task<IEnumerable<PickingWave>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<PickingWave>> GetAllAsync();
    Task AddAsync(PickingWave wave);
    Task UpdateAsync(PickingWave wave);
    Task DeleteAsync(Guid id);
}
