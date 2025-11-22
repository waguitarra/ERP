using Logistics.Application.DTOs.DockDoor;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DockDoorsController : ControllerBase
{
    private readonly IDockDoorService _service;

    public DockDoorsController(IDockDoorService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<DockDoorResponse>> Create([FromBody] CreateDockDoorRequest request)
    {
        var dockDoor = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = dockDoor.Id }, dockDoor);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DockDoorResponse>> GetById(Guid id)
    {
        var dockDoor = await _service.GetByIdAsync(id);
        return Ok(dockDoor);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<List<DockDoorResponse>>> GetByWarehouse(Guid warehouseId)
    {
        var dockDoors = await _service.GetByWarehouseIdAsync(warehouseId);
        return Ok(dockDoors.ToList());
    }

    [HttpGet("warehouse/{warehouseId}/available")]
    public async Task<ActionResult<List<DockDoorResponse>>> GetAvailable(Guid warehouseId)
    {
        var dockDoors = await _service.GetAvailableAsync(warehouseId);
        return Ok(dockDoors.ToList());
    }

    [HttpGet]
    public async Task<ActionResult<List<DockDoorResponse>>> GetAll()
    {
        var dockDoors = await _service.GetAllAsync();
        return Ok(dockDoors.ToList());
    }
}
