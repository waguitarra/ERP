using Logistics.Application.DTOs.Lot;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotsController : ControllerBase
{
    private readonly ILotService _service;

    public LotsController(ILotService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<LotResponse>> Create([FromBody] CreateLotRequest request)
    {
        var lot = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = lot.Id }, lot);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LotResponse>> GetById(Guid id)
    {
        var lot = await _service.GetByIdAsync(id);
        return Ok(lot);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<List<LotResponse>>> GetByProduct(Guid productId)
    {
        var lots = await _service.GetByProductIdAsync(productId);
        return Ok(lots.ToList());
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<List<LotResponse>>> GetByCompany(Guid companyId)
    {
        var lots = await _service.GetByCompanyIdAsync(companyId);
        return Ok(lots.ToList());
    }

    [HttpGet("company/{companyId}/expiring")]
    public async Task<ActionResult<List<LotResponse>>> GetExpiring(Guid companyId, [FromQuery] int daysAhead = 30)
    {
        var lots = await _service.GetExpiringLotsAsync(companyId, daysAhead);
        return Ok(lots.ToList());
    }

    [HttpPost("{id}/quarantine")]
    public async Task<ActionResult> Quarantine(Guid id)
    {
        await _service.QuarantineLotAsync(id);
        return Ok();
    }

    [HttpPost("{id}/release")]
    public async Task<ActionResult> Release(Guid id)
    {
        await _service.ReleaseLotAsync(id);
        return Ok();
    }
}
