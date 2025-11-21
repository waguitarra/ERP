using Logistics.Application.DTOs.Warehouse;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
namespace Logistics.Application.Services;
public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    public WarehouseService(IWarehouseRepository warehouseRepository, ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _warehouseRepository = warehouseRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<WarehouseResponse> CreateAsync(WarehouseRequest request)
    {
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            throw new KeyNotFoundException("Empresa não encontrada");
        if (await _warehouseRepository.CodeExistsAsync(request.Code))
            throw new InvalidOperationException("Código já existe");
        var warehouse = new Warehouse(request.CompanyId, request.Name, request.Code, request.Address);
        await _warehouseRepository.AddAsync(warehouse);
        await _unitOfWork.CommitAsync();
        return MapToResponse(warehouse);
    }
    public async Task<WarehouseResponse> GetByIdAsync(Guid id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null) throw new KeyNotFoundException("Armazém não encontrado");
        return MapToResponse(warehouse);
    }
    public async Task<IEnumerable<WarehouseResponse>> GetAllAsync()
    {
        var warehouses = await _warehouseRepository.GetAllAsync();
        return warehouses.Select(MapToResponse);
    }
    public async Task<IEnumerable<WarehouseResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var warehouses = await _warehouseRepository.GetByCompanyIdAsync(companyId);
        return warehouses.Select(MapToResponse);
    }
    public async Task<WarehouseResponse> UpdateAsync(Guid id, WarehouseRequest request)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null) throw new KeyNotFoundException("Armazém não encontrado");
        if (await _warehouseRepository.CodeExistsAsync(request.Code, id))
            throw new InvalidOperationException("Código já existe");
        warehouse.Update(request.Name, request.Code, request.Address);
        await _warehouseRepository.UpdateAsync(warehouse);
        await _unitOfWork.CommitAsync();
        return MapToResponse(warehouse);
    }
    public async Task DeleteAsync(Guid id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null) throw new KeyNotFoundException("Armazém não encontrado");
        await _warehouseRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
    private static WarehouseResponse MapToResponse(Warehouse w) => new WarehouseResponse
    {
        Id = w.Id, CompanyId = w.CompanyId, Name = w.Name, Code = w.Code,
        Address = w.Address, IsActive = w.IsActive, CreatedAt = w.CreatedAt
    };
}
