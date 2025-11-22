using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.VehicleAppointment;

public record CreateVehicleAppointmentRequest(
    string AppointmentNumber,
    Guid WarehouseId,
    Guid? VehicleId,
    Guid? DriverId,
    AppointmentType Type,
    DateTime ScheduledDate,
    Guid? DockDoorId
);
