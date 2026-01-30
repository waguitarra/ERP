using Logistics.Application.DTOs.OutboundShipment;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/outbound-shipments")]
[Authorize]
public class OutboundShipmentsController : ControllerBase
{
    private readonly IOutboundShipmentService _service;

    public OutboundShipmentsController(IOutboundShipmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var shipments = await _service.GetAllAsync();
        return Ok(new { success = true, data = shipments.ToList() });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var shipment = await _service.GetByIdAsync(id);
        return Ok(new { success = true, data = shipment });
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult> GetByOrder(Guid orderId)
    {
        var shipments = await _service.GetByOrderIdAsync(orderId);
        return Ok(new { success = true, data = shipments.ToList() });
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult> GetByStatus(OutboundStatus status)
    {
        var shipments = await _service.GetByStatusAsync(status);
        return Ok(new { success = true, data = shipments.ToList() });
    }

    [HttpGet("pending")]
    public async Task<ActionResult> GetPending()
    {
        var shipments = await _service.GetPendingAsync();
        return Ok(new { success = true, data = shipments.ToList() });
    }

    [HttpGet("shipped")]
    public async Task<ActionResult> GetShipped()
    {
        var shipments = await _service.GetShippedAsync();
        return Ok(new { success = true, data = shipments.ToList() });
    }

    [HttpGet("in-transit")]
    public async Task<ActionResult> GetInTransit()
    {
        var shipments = await _service.GetInTransitAsync();
        return Ok(new { success = true, data = shipments.ToList() });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateOutboundShipmentRequest request)
    {
        var shipment = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, new { success = true, data = shipment });
    }

    [HttpPost("{id}/mark-ready")]
    public async Task<ActionResult> MarkReadyToShip(Guid id)
    {
        await _service.MarkReadyToShipAsync(id);
        return Ok(new { success = true, message = "Expedição marcada como pronta para envio" });
    }

    [HttpPost("{id}/ship")]
    public async Task<ActionResult> Ship(Guid id)
    {
        await _service.ShipAsync(id);
        return Ok(new { success = true, message = "Expedição enviada com sucesso" });
    }

    [HttpPost("{id}/in-transit")]
    public async Task<ActionResult> MarkInTransit(Guid id)
    {
        await _service.MarkInTransitAsync(id);
        return Ok(new { success = true, message = "Expedição marcada como em trânsito" });
    }

    [HttpPost("{id}/deliver")]
    public async Task<ActionResult> Deliver(Guid id)
    {
        await _service.DeliverAsync(id);
        return Ok(new { success = true, message = "Expedição entregue com sucesso" });
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> Cancel(Guid id)
    {
        await _service.CancelAsync(id);
        return Ok(new { success = true, message = "Expedição cancelada com sucesso" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { success = true, message = "Expedição excluída com sucesso" });
    }
}
