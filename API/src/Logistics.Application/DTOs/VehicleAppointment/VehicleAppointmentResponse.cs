using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.VehicleAppointment;

public record VehicleAppointmentResponse(
    Guid Id,
    string AppointmentNumber,
    Guid WarehouseId,
    string WarehouseName,
    Guid? VehicleId,
    string? VehiclePlate,
    Guid? DriverId,
    string? DriverName,
    Guid? DockDoorId,
    string? DockDoorNumber,
    AppointmentType Type,
    DateTime ScheduledDate,
    DateTime? ArrivalDate,
    DateTime? DepartureDate,
    AppointmentStatus Status,
    DateTime CreatedAt
);
