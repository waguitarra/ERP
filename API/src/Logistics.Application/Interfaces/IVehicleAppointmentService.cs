using Logistics.Application.DTOs.VehicleAppointment;

namespace Logistics.Application.Interfaces;

public interface IVehicleAppointmentService
{
    Task<VehicleAppointmentResponse> CreateAsync(CreateVehicleAppointmentRequest request);
    Task<VehicleAppointmentResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<VehicleAppointmentResponse>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<VehicleAppointmentResponse>> GetAllAsync();
    Task CheckInAsync(Guid id);
    Task CheckOutAsync(Guid id);
}
