using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class VehicleAppointmentRepository : IVehicleAppointmentRepository
{
    private readonly LogisticsDbContext _context;

    public VehicleAppointmentRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleAppointment?> GetByIdAsync(Guid id)
    {
        return await _context.VehicleAppointments
            .Include(a => a.Vehicle)
            .Include(a => a.Driver)
            .Include(a => a.DockDoor)
            .Include(a => a.Warehouse)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<VehicleAppointment?> GetByAppointmentNumberAsync(string appointmentNumber)
    {
        return await _context.VehicleAppointments
            .FirstOrDefaultAsync(a => a.AppointmentNumber == appointmentNumber);
    }

    public async Task<IEnumerable<VehicleAppointment>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _context.VehicleAppointments
            .Include(a => a.Vehicle)
            .Include(a => a.Driver)
            .Where(a => a.WarehouseId == warehouseId)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<VehicleAppointment>> GetByDateAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        
        return await _context.VehicleAppointments
            .Include(a => a.Vehicle)
            .Where(a => a.ScheduledDate >= startOfDay && a.ScheduledDate < endOfDay)
            .OrderBy(a => a.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<VehicleAppointment>> GetAllAsync()
    {
        return await _context.VehicleAppointments
            .Include(a => a.Vehicle)
            .Include(a => a.Driver)
            .ToListAsync();
    }

    public async Task AddAsync(VehicleAppointment appointment)
    {
        await _context.VehicleAppointments.AddAsync(appointment);
    }

    public Task UpdateAsync(VehicleAppointment appointment)
    {
        _context.VehicleAppointments.Update(appointment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var appointment = await GetByIdAsync(id);
        if (appointment != null)
            _context.VehicleAppointments.Remove(appointment);
    }
}
