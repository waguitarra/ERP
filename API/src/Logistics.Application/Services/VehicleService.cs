using Logistics.Application.DTOs.Vehicle;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleResponse> CreateAsync(VehicleRequest request)
    {
        // Valida se a empresa existe
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
            throw new KeyNotFoundException($"Empresa com ID {request.CompanyId} não encontrada");

        // Valida se a placa já existe
        if (await _vehicleRepository.LicensePlateExistsAsync(request.LicensePlate))
            throw new InvalidOperationException($"Já existe um veículo com a placa {request.LicensePlate}");

        var vehicle = new Vehicle(
            request.CompanyId,
            request.LicensePlate,
            request.Model,
            request.Year
        );

        await _vehicleRepository.AddAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> GetByIdAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        return MapToResponse(vehicle);
    }

    public async Task<IEnumerable<VehicleResponse>> GetAllAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return vehicles.Select(MapToResponse);
    }

    public async Task<IEnumerable<VehicleResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var vehicles = await _vehicleRepository.GetByCompanyIdAsync(companyId);
        return vehicles.Select(MapToResponse);
    }

    public async Task<VehicleResponse> UpdateAsync(Guid id, VehicleRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        // Valida se a empresa existe
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
            throw new KeyNotFoundException($"Empresa com ID {request.CompanyId} não encontrada");

        // Valida se a placa já existe em outro veículo
        if (await _vehicleRepository.LicensePlateExistsAsync(request.LicensePlate, id))
            throw new InvalidOperationException($"Já existe outro veículo com a placa {request.LicensePlate}");

        vehicle.Update(request.LicensePlate, request.Model, request.Year);

        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    public async Task DeleteAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        await _vehicleRepository.DeleteAsync(vehicle.Id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<VehicleResponse> UpdateStatusAsync(Guid id, string status)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        if (!Enum.TryParse<VehicleStatus>(status, true, out var vehicleStatus))
            throw new ArgumentException($"Status '{status}' inválido. Valores válidos: Available, InUse, Maintenance, Inactive");

        vehicle.UpdateStatus(vehicleStatus);

        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    private static VehicleResponse MapToResponse(Vehicle vehicle)
    {
        return new VehicleResponse
        {
            Id = vehicle.Id,
            CompanyId = vehicle.CompanyId,
            LicensePlate = vehicle.LicensePlate,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Status = vehicle.Status.ToString(),
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt,
            CompanyName = vehicle.Company?.Name
        };
    }
}
