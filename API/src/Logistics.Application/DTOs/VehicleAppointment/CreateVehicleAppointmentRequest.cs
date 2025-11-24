using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.VehicleAppointment;

public class CreateVehicleAppointmentRequest
{
    public string AppointmentNumber { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public AppointmentType Type { get; set; }
    public DateTime ScheduledDate { get; set; }
    public Guid? DockDoorId { get; set; }
}
