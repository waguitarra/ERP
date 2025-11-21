using Logistics.Application.DTOs.Company;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CompanyResponse> CreateAsync(CompanyRequest request)
    {
        if (await _unitOfWork.Companies.DocumentExistsAsync(request.Document))
            throw new InvalidOperationException("Documento já cadastrado");

        var company = new Company(request.Name, request.Document);

        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.CommitAsync();

        return MapToResponse(company);
    }

    public async Task<CompanyResponse> GetByIdAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        
        if (company == null)
            throw new KeyNotFoundException("Empresa não encontrada");

        return MapToResponse(company);
    }

    public async Task<IEnumerable<CompanyResponse>> GetAllAsync()
    {
        var companies = await _unitOfWork.Companies.GetAllAsync();
        return companies.Select(MapToResponse);
    }

    public async Task<CompanyResponse> UpdateAsync(Guid id, CompanyRequest request)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        
        if (company == null)
            throw new KeyNotFoundException("Empresa não encontrada");

        if (await _unitOfWork.Companies.DocumentExistsAsync(request.Document, id))
            throw new InvalidOperationException("Documento já cadastrado para outra empresa");

        company.Update(request.Name, request.Document);

        await _unitOfWork.Companies.UpdateAsync(company);
        await _unitOfWork.CommitAsync();

        return MapToResponse(company);
    }

    public async Task DeleteAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        
        if (company == null)
            throw new KeyNotFoundException("Empresa não encontrada");

        company.Deactivate();
        
        await _unitOfWork.Companies.UpdateAsync(company);
        await _unitOfWork.CommitAsync();
    }

    private static CompanyResponse MapToResponse(Company company)
    {
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Document = company.Document,
            IsActive = company.IsActive,
            CreatedAt = company.CreatedAt
        };
    }
}
