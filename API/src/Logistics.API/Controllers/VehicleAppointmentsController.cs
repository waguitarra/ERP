using Logistics.Application.DTOs.VehicleAppointment;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleAppointmentsController : ControllerBase
{
    private readonly IVehicleAppointmentService _service;

    public VehicleAppointmentsController(IVehicleAppointmentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<VehicleAppointmentResponse>> Create([FromBody] CreateVehicleAppointmentRequest request)
    {
        var appointment = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleAppointmentResponse>> GetById(Guid id)
    {
        var appointment = await _service.GetByIdAsync(id);
        return Ok(appointment);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<List<VehicleAppointmentResponse>>> GetByWarehouse(Guid warehouseId)
    {
        var appointments = await _service.GetByWarehouseIdAsync(warehouseId);
        return Ok(appointments.ToList());
    }

    [HttpGet]
    public async Task<ActionResult<List<VehicleAppointmentResponse>>> GetAll()
    {
        var appointments = await _service.GetAllAsync();
        return Ok(appointments.ToList());
    }

    [HttpPost("{id}/checkin")]
    public async Task<ActionResult> CheckIn(Guid id)
    {
        await _service.CheckInAsync(id);
        return Ok();
    }

    [HttpPost("{id}/checkout")]
    public async Task<ActionResult> CheckOut(Guid id)
    {
        await _service.CheckOutAsync(id);
        return Ok();
    }
}
