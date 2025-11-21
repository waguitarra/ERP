using Logistics.Application.DTOs.Warehouse;
namespace Logistics.Application.Interfaces;
public interface IWarehouseService
{
    Task<WarehouseResponse> CreateAsync(WarehouseRequest request);
    Task<WarehouseResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<WarehouseResponse>> GetAllAsync();
    Task<IEnumerable<WarehouseResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<WarehouseResponse> UpdateAsync(Guid id, WarehouseRequest request);
    Task DeleteAsync(Guid id);
}
