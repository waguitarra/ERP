using Logistics.Application.DTOs.CycleCount;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CycleCountsController : ControllerBase
{
    private readonly ICycleCountService _service;

    public CycleCountsController(ICycleCountService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<CycleCountResponse>> Create([FromBody] CreateCycleCountRequest request)
    {
        var cycleCount = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = cycleCount.Id }, cycleCount);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CycleCountResponse>> GetById(Guid id)
    {
        var cycleCount = await _service.GetByIdAsync(id);
        return Ok(cycleCount);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<List<CycleCountResponse>>> GetByWarehouse(Guid warehouseId)
    {
        var cycleCounts = await _service.GetByWarehouseIdAsync(warehouseId);
        return Ok(cycleCounts.ToList());
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult> Complete(Guid id)
    {
        await _service.CompleteAsync(id);
        return Ok();
    }
}
