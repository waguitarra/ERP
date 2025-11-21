using Logistics.Application.DTOs.Driver;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DriverService(
        IDriverRepository driverRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DriverResponse> CreateAsync(DriverRequest request)
    {
        // Valida se a empresa existe
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
            throw new KeyNotFoundException($"Empresa com ID {request.CompanyId} não encontrada");

        // Valida se a CNH já existe
        if (await _driverRepository.LicenseNumberExistsAsync(request.LicenseNumber))
            throw new InvalidOperationException($"Já existe um motorista com a CNH {request.LicenseNumber}");

        var driver = new Driver(
            request.CompanyId,
            request.Name,
            request.LicenseNumber,
            request.Phone
        );

        await _driverRepository.AddAsync(driver);
        await _unitOfWork.CommitAsync();

        return MapToResponse(driver);
    }

    public async Task<DriverResponse> GetByIdAsync(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        if (driver == null)
            throw new KeyNotFoundException($"Motorista com ID {id} não encontrado");

        return MapToResponse(driver);
    }

    public async Task<IEnumerable<DriverResponse>> GetAllAsync()
    {
        var drivers = await _driverRepository.GetAllAsync();
        return drivers.Select(MapToResponse);
    }

    public async Task<IEnumerable<DriverResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var drivers = await _driverRepository.GetByCompanyIdAsync(companyId);
        return drivers.Select(MapToResponse);
    }

    public async Task<DriverResponse> UpdateAsync(Guid id, DriverRequest request)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        if (driver == null)
            throw new KeyNotFoundException($"Motorista com ID {id} não encontrado");

        // Valida se a empresa existe
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
            throw new KeyNotFoundException($"Empresa com ID {request.CompanyId} não encontrada");

        // Valida se a CNH já existe em outro motorista
        if (await _driverRepository.LicenseNumberExistsAsync(request.LicenseNumber, id))
            throw new InvalidOperationException($"Já existe outro motorista com a CNH {request.LicenseNumber}");

        driver.Update(request.Name, request.LicenseNumber, request.Phone);

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CommitAsync();

        return MapToResponse(driver);
    }

    public async Task DeleteAsync(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        if (driver == null)
            throw new KeyNotFoundException($"Motorista com ID {id} não encontrado");

        await _driverRepository.DeleteAsync(driver.Id);
        await _unitOfWork.CommitAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        if (driver == null)
            throw new KeyNotFoundException($"Motorista com ID {id} não encontrado");

        driver.Activate();

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var driver = await _driverRepository.GetByIdAsync(id);
        if (driver == null)
            throw new KeyNotFoundException($"Motorista com ID {id} não encontrado");

        driver.Deactivate();

        await _driverRepository.UpdateAsync(driver);
        await _unitOfWork.CommitAsync();
    }

    private static DriverResponse MapToResponse(Driver driver)
    {
        return new DriverResponse
        {
            Id = driver.Id,
            CompanyId = driver.CompanyId,
            Name = driver.Name,
            LicenseNumber = driver.LicenseNumber,
            Phone = driver.Phone,
            IsActive = driver.IsActive,
            CreatedAt = driver.CreatedAt,
            UpdatedAt = driver.UpdatedAt,
            CompanyName = driver.Company?.Name
        };
    }
}
