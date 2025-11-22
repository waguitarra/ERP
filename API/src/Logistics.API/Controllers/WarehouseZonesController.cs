using Logistics.Application.DTOs.WarehouseZone;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehouseZonesController : ControllerBase
{
    private readonly IWarehouseZoneService _service;

    public WarehouseZonesController(IWarehouseZoneService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseZoneResponse>> Create([FromBody] CreateWarehouseZoneRequest request)
    {
        var zone = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<List<WarehouseZoneResponse>>> GetByWarehouse(Guid warehouseId)
    {
        var zones = await _service.GetByWarehouseIdAsync(warehouseId);
        return Ok(zones.ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WarehouseZoneResponse>> GetById(Guid id)
    {
        var zone = await _service.GetByIdAsync(id);
        if (zone == null)
            return NotFound();

        return Ok(zone);
    }
}
