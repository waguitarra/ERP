using Logistics.Application.DTOs.PickingWave;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PickingWavesController : ControllerBase
{
    private readonly IPickingWaveService _service;

    public PickingWavesController(IPickingWaveService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<PickingWaveResponse>> Create([FromBody] CreatePickingWaveRequest request)
    {
        var wave = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = wave.Id }, wave);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PickingWaveResponse>> GetById(Guid id)
    {
        var wave = await _service.GetByIdAsync(id);
        return Ok(wave);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<List<PickingWaveResponse>>> GetByWarehouse(Guid warehouseId)
    {
        var waves = await _service.GetByWarehouseIdAsync(warehouseId);
        return Ok(waves.ToList());
    }

    [HttpGet]
    public async Task<ActionResult<List<PickingWaveResponse>>> GetAll()
    {
        var waves = await _service.GetAllAsync();
        return Ok(waves.ToList());
    }

    [HttpPost("{id}/release")]
    public async Task<ActionResult> Release(Guid id)
    {
        await _service.ReleaseAsync(id);
        return Ok();
    }
}
