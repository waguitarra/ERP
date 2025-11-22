using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IVehicleAppointmentRepository
{
    Task<VehicleAppointment?> GetByIdAsync(Guid id);
    Task<VehicleAppointment?> GetByAppointmentNumberAsync(string appointmentNumber);
    Task<IEnumerable<VehicleAppointment>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<IEnumerable<VehicleAppointment>> GetByDateAsync(DateTime date);
    Task<IEnumerable<VehicleAppointment>> GetAllAsync();
    Task AddAsync(VehicleAppointment appointment);
    Task UpdateAsync(VehicleAppointment appointment);
    Task DeleteAsync(Guid id);
}
