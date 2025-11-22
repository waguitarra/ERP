using Logistics.Application.DTOs.InboundShipment;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InboundShipmentsController : ControllerBase
{
    private readonly IInboundShipmentService _service;

    public InboundShipmentsController(IInboundShipmentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<InboundShipmentResponse>> Create([FromBody] CreateInboundShipmentRequest request)
    {
        var shipment = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InboundShipmentResponse>> GetById(Guid id)
    {
        var shipment = await _service.GetByIdAsync(id);
        return Ok(shipment);
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<List<InboundShipmentResponse>>> GetByCompany(Guid companyId)
    {
        var shipments = await _service.GetByCompanyIdAsync(companyId);
        return Ok(shipments.ToList());
    }

    [HttpGet]
    public async Task<ActionResult<List<InboundShipmentResponse>>> GetAll()
    {
        var shipments = await _service.GetAllAsync();
        return Ok(shipments.ToList());
    }

    [HttpPost("{id}/receive")]
    public async Task<ActionResult> Receive(Guid id, [FromBody] Guid receivedBy)
    {
        await _service.ReceiveAsync(id, receivedBy);
        return Ok();
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult> Complete(Guid id)
    {
        await _service.CompleteAsync(id);
        return Ok();
    }
}
