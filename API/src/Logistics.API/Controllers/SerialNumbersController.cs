using Logistics.Application.DTOs.SerialNumber;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SerialNumbersController : ControllerBase
{
    private readonly ISerialNumberService _service;

    public SerialNumbersController(ISerialNumberService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<SerialNumberResponse>> Create([FromBody] CreateSerialNumberRequest request)
    {
        var serialNumber = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = serialNumber.Id }, serialNumber);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SerialNumberResponse>> GetById(Guid id)
    {
        var serialNumber = await _service.GetByIdAsync(id);
        return Ok(serialNumber);
    }

    [HttpGet("serial/{serial}")]
    public async Task<ActionResult<SerialNumberResponse>> GetBySerial(string serial)
    {
        var serialNumber = await _service.GetBySerialAsync(serial);
        return Ok(serialNumber);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<List<SerialNumberResponse>>> GetByProduct(Guid productId)
    {
        var serialNumbers = await _service.GetByProductIdAsync(productId);
        return Ok(serialNumbers.ToList());
    }

    [HttpGet("lot/{lotId}")]
    public async Task<ActionResult<List<SerialNumberResponse>>> GetByLot(Guid lotId)
    {
        var serialNumbers = await _service.GetByLotIdAsync(lotId);
        return Ok(serialNumbers.ToList());
    }

    [HttpPost("{id}/receive")]
    public async Task<ActionResult> Receive(Guid id, [FromBody] Guid locationId)
    {
        await _service.ReceiveAsync(id, locationId);
        return Ok();
    }

    [HttpPost("{id}/ship")]
    public async Task<ActionResult> Ship(Guid id)
    {
        await _service.ShipAsync(id);
        return Ok();
    }
}
