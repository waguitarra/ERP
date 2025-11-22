using Logistics.Application.DTOs.VehicleAppointment;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class VehicleAppointmentService : IVehicleAppointmentService
{
    private readonly IVehicleAppointmentRepository _repository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VehicleAppointmentService(
        IVehicleAppointmentRepository repository,
        IWarehouseRepository warehouseRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleAppointmentResponse> CreateAsync(CreateVehicleAppointmentRequest request)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId);
        if (warehouse == null) throw new KeyNotFoundException("Armazém não encontrado");

        var appointment = new VehicleAppointment(
            request.AppointmentNumber,
            request.WarehouseId,
            request.Type,
            request.ScheduledDate
        );

        if (request.VehicleId.HasValue || request.DriverId.HasValue)
        {
            appointment.SetVehicleAndDriver(request.VehicleId, request.DriverId);
        }

        if (request.DockDoorId.HasValue)
        {
            appointment.AssignDockDoor(request.DockDoorId.Value);
        }

        await _repository.AddAsync(appointment);
        await _unitOfWork.CommitAsync();

        return await GetByIdAsync(appointment.Id);
    }

    public async Task<VehicleAppointmentResponse> GetByIdAsync(Guid id)
    {
        var appointment = await _repository.GetByIdAsync(id);
        if (appointment == null) throw new KeyNotFoundException("Agendamento não encontrado");
        return MapToResponse(appointment);
    }

    public async Task<IEnumerable<VehicleAppointmentResponse>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        var appointments = await _repository.GetByWarehouseIdAsync(warehouseId);
        return appointments.Select(MapToResponse);
    }

    public async Task<IEnumerable<VehicleAppointmentResponse>> GetAllAsync()
    {
        var appointments = await _repository.GetAllAsync();
        return appointments.Select(MapToResponse);
    }

    public async Task CheckInAsync(Guid id)
    {
        var appointment = await _repository.GetByIdAsync(id);
        if (appointment == null) throw new KeyNotFoundException("Agendamento não encontrado");

        appointment.CheckIn(DateTime.UtcNow);
        await _unitOfWork.CommitAsync();
    }

    public async Task CheckOutAsync(Guid id)
    {
        var appointment = await _repository.GetByIdAsync(id);
        if (appointment == null) throw new KeyNotFoundException("Agendamento não encontrado");

        appointment.CheckOut(DateTime.UtcNow);
        await _unitOfWork.CommitAsync();
    }

    private static VehicleAppointmentResponse MapToResponse(VehicleAppointment appointment)
    {
        return new VehicleAppointmentResponse(
            appointment.Id,
            appointment.AppointmentNumber,
            appointment.WarehouseId,
            appointment.Warehouse?.Name ?? "",
            appointment.VehicleId,
            appointment.Vehicle?.LicensePlate,
            appointment.DriverId,
            appointment.Driver?.Name,
            appointment.DockDoorId,
            appointment.DockDoor?.DoorNumber,
            appointment.Type,
            appointment.ScheduledDate,
            appointment.ArrivalDate,
            appointment.DepartureDate,
            appointment.Status,
            appointment.CreatedAt
        );
    }
}
