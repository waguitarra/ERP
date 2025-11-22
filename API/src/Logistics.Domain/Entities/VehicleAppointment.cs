using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class VehicleAppointment
{
    private VehicleAppointment() { } // EF Core

    public VehicleAppointment(string appointmentNumber, Guid warehouseId, AppointmentType type, DateTime scheduledDate)
    {
        if (string.IsNullOrWhiteSpace(appointmentNumber)) throw new ArgumentException("AppointmentNumber inválido");
        if (warehouseId == Guid.Empty) throw new ArgumentException("WarehouseId inválido");

        Id = Guid.NewGuid();
        AppointmentNumber = appointmentNumber;
        WarehouseId = warehouseId;
        Type = type;
        ScheduledDate = scheduledDate;
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string AppointmentNumber { get; private set; } = string.Empty;
    public Guid WarehouseId { get; private set; }
    public Guid? VehicleId { get; private set; }
    public Guid? DriverId { get; private set; }
    public Guid? DockDoorId { get; private set; }
    public AppointmentType Type { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public DateTime? ArrivalDate { get; private set; }
    public DateTime? DepartureDate { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public TimeSpan? ServiceTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public Warehouse Warehouse { get; private set; } = null!;
    public Vehicle? Vehicle { get; private set; }
    public Driver? Driver { get; private set; }
    public DockDoor? DockDoor { get; private set; }

    public void SetVehicleAndDriver(Guid? vehicleId, Guid? driverId)
    {
        VehicleId = vehicleId;
        DriverId = driverId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignDockDoor(Guid dockDoorId)
    {
        DockDoorId = dockDoorId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CheckIn(DateTime arrivalDate)
    {
        ArrivalDate = arrivalDate;
        Status = AppointmentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CheckOut(DateTime departureDate)
    {
        DepartureDate = departureDate;
        Status = AppointmentStatus.Completed;
        if (ArrivalDate.HasValue)
        {
            ServiceTime = departureDate - ArrivalDate.Value;
        }
        UpdatedAt = DateTime.UtcNow;
    }
}
