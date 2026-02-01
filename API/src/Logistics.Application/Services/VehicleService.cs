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
            request.Year,
            request.Brand,
            request.VehicleType,
            request.Capacity,
            request.Color,
            request.FuelType
        );

        if (request.TrackingEnabled)
            vehicle.EnableTracking();

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

        vehicle.Update(request.LicensePlate, request.Model, request.Year, request.Brand,
                       request.VehicleType, request.Capacity, request.Color, request.FuelType, request.Notes);

        if (request.TrackingEnabled && !vehicle.TrackingEnabled)
            vehicle.EnableTracking();
        else if (!request.TrackingEnabled && vehicle.TrackingEnabled)
            vehicle.DisableTracking();

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

    public async Task<VehicleResponse> EnableTrackingAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        vehicle.EnableTracking();
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> DisableTrackingAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        vehicle.DisableTracking();
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> RegenerateTrackingTokenAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        vehicle.RegenerateTrackingToken();
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> UpdateLocationAsync(Guid id, UpdateVehicleLocationRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        if (!vehicle.TrackingEnabled)
            throw new InvalidOperationException("Rastreamento não está habilitado para este veículo");

        vehicle.UpdateLocation(request.Latitude, request.Longitude, request.Speed, request.Address);
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> AssignDriverAsync(Guid id, AssignDriverRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        vehicle.AssignDriver(request.DriverName, request.DriverPhone);
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse> RemoveDriverAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null)
            throw new KeyNotFoundException($"Veículo com ID {id} não encontrado");

        vehicle.RemoveDriver();
        await _vehicleRepository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();

        return MapToResponse(vehicle);
    }

    public async Task<IEnumerable<VehicleResponse>> GetWithTrackingEnabledAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return vehicles.Where(v => v.TrackingEnabled).Select(MapToResponse);
    }

    private static VehicleResponse MapToResponse(Vehicle vehicle)
    {
        return new VehicleResponse
        {
            Id = vehicle.Id,
            CompanyId = vehicle.CompanyId,
            LicensePlate = vehicle.LicensePlate,
            Model = vehicle.Model,
            Brand = vehicle.Brand,
            VehicleType = vehicle.VehicleType,
            Year = vehicle.Year,
            Capacity = vehicle.Capacity,
            Status = (int)vehicle.Status,
            StatusName = vehicle.Status.ToString(),
            TrackingToken = vehicle.TrackingToken,
            TrackingEnabled = vehicle.TrackingEnabled,
            LastLatitude = vehicle.LastLatitude,
            LastLongitude = vehicle.LastLongitude,
            LastLocationUpdate = vehicle.LastLocationUpdate,
            CurrentSpeed = vehicle.CurrentSpeed,
            CurrentAddress = vehicle.CurrentAddress,
            IsMoving = vehicle.IsMoving,
            
            // Mileage info
            CurrentMileage = vehicle.CurrentMileage,
            TotalDistanceTraveled = vehicle.TotalDistanceTraveled,
            
            // Financial info
            PurchasePrice = vehicle.PurchasePrice,
            PurchaseDate = vehicle.PurchaseDate,
            CurrentValue = vehicle.CurrentValue,
            ChassisNumber = vehicle.ChassisNumber,
            EngineNumber = vehicle.EngineNumber,
            
            // Insurance & Documentation
            InsuranceExpiryDate = vehicle.InsuranceExpiryDate,
            LicenseExpiryDate = vehicle.LicenseExpiryDate,
            LastInspectionDate = vehicle.LastInspectionDate,
            NextInspectionDate = vehicle.NextInspectionDate,
            IsInsuranceExpired = vehicle.IsInsuranceExpired,
            IsInsuranceExpiringSoon = vehicle.IsInsuranceExpiringSoon,
            IsInspectionExpired = vehicle.IsInspectionExpired,
            IsInspectionExpiringSoon = vehicle.IsInspectionExpiringSoon,
            
            // Maintenance info
            LastMaintenanceDate = vehicle.LastMaintenanceDate,
            LastMaintenanceMileage = vehicle.LastMaintenanceMileage,
            TotalMaintenanceCost = vehicle.TotalMaintenanceCost,
            MaintenanceCount = vehicle.Maintenances?.Count ?? 0,
            DocumentCount = vehicle.Documents?.Count ?? 0,
            InspectionCount = vehicle.Inspections?.Count ?? 0,
            DeliveryCount = vehicle.DeliveryHistory?.Count ?? 0,
            
            // Driver info
            DriverId = vehicle.DriverId,
            DriverName = vehicle.DriverName,
            DriverPhone = vehicle.DriverPhone,
            DriverLicenseNumber = vehicle.Driver?.LicenseNumber,
            
            // Current shipment
            CurrentShipmentId = vehicle.CurrentShipmentId,
            CurrentShipmentNumber = vehicle.CurrentShipment?.ShipmentNumber,
            CurrentShipmentAddress = vehicle.CurrentShipment?.DeliveryAddress,
            CurrentShipmentStatus = vehicle.CurrentShipment?.Status.ToString(),
            CurrentOrderCustomerName = vehicle.CurrentShipment?.Order?.Customer?.Name,
            CurrentOrderCustomerPhone = vehicle.CurrentShipment?.Order?.Customer?.Phone,
            
            Color = vehicle.Color,
            FuelType = vehicle.FuelType,
            Notes = vehicle.Notes,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt,
            CompanyName = vehicle.Company?.Name
        };
    }
}
