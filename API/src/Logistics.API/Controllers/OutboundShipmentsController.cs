using Logistics.Application.DTOs.OutboundShipment;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutboundShipmentsController : ControllerBase
{
    private readonly IOutboundShipmentService _service;

    public OutboundShipmentsController(IOutboundShipmentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<OutboundShipmentResponse>> Create([FromBody] CreateOutboundShipmentRequest request)
    {
        var shipment = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OutboundShipmentResponse>> GetById(Guid id)
    {
        var shipment = await _service.GetByIdAsync(id);
        return Ok(shipment);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<List<OutboundShipmentResponse>>> GetByOrder(Guid orderId)
    {
        var shipments = await _service.GetByOrderIdAsync(orderId);
        return Ok(shipments.ToList());
    }

    [HttpPost("{id}/ship")]
    public async Task<ActionResult> Ship(Guid id)
    {
        await _service.ShipAsync(id);
        return Ok();
    }

    [HttpPost("{id}/deliver")]
    public async Task<ActionResult> Deliver(Guid id)
    {
        await _service.DeliverAsync(id);
        return Ok();
    }
}
