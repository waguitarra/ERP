using Logistics.Application.DTOs.Supplier;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
namespace Logistics.Application.Services;
public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    public SupplierService(ISupplierRepository supplierRepository, ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<SupplierResponse> CreateAsync(SupplierRequest request)
    {
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            throw new KeyNotFoundException("Empresa não encontrada");
        if (await _supplierRepository.DocumentExistsAsync(request.Document))
            throw new InvalidOperationException("Documento já existe");
        var supplier = new Supplier(request.CompanyId, request.Name, request.Document, request.Phone, request.Email);
        supplier.Update(request.Name, request.Document, request.Phone, request.Email, request.Address);
        await _supplierRepository.AddAsync(supplier);
        await _unitOfWork.CommitAsync();
        return MapToResponse(supplier);
    }
    public async Task<SupplierResponse> GetByIdAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null) throw new KeyNotFoundException("Fornecedor não encontrado");
        return MapToResponse(supplier);
    }
    public async Task<IEnumerable<SupplierResponse>> GetAllAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(MapToResponse);
    }
    public async Task<IEnumerable<SupplierResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var suppliers = await _supplierRepository.GetByCompanyIdAsync(companyId);
        return suppliers.Select(MapToResponse);
    }
    public async Task<SupplierResponse> UpdateAsync(Guid id, SupplierRequest request)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null) throw new KeyNotFoundException("Fornecedor não encontrado");
        if (await _supplierRepository.DocumentExistsAsync(request.Document, id))
            throw new InvalidOperationException("Documento já existe");
        supplier.Update(request.Name, request.Document, request.Phone, request.Email, request.Address);
        await _supplierRepository.UpdateAsync(supplier);
        await _unitOfWork.CommitAsync();
        return MapToResponse(supplier);
    }
    public async Task DeleteAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null) throw new KeyNotFoundException("Fornecedor não encontrado");
        await _supplierRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
    private static SupplierResponse MapToResponse(Supplier s) => new SupplierResponse
    {
        Id = s.Id, CompanyId = s.CompanyId, Name = s.Name, Document = s.Document,
        Phone = s.Phone, Email = s.Email, Address = s.Address,
        IsActive = s.IsActive, CreatedAt = s.CreatedAt
    };
}
