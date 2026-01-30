using Logistics.Application.DTOs.InboundShipment;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/inbound-shipments")]
[Authorize]
public class InboundShipmentsController : ControllerBase
{
    private readonly IInboundShipmentService _service;

    public InboundShipmentsController(IInboundShipmentService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtém todos os recebimentos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetAll()
    {
        var shipments = await _service.GetAllAsync();
        return Ok(new { success = true, data = shipments });
    }

    /// <summary>
    /// Obtém um recebimento por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(Guid id)
    {
        var shipment = await _service.GetByIdAsync(id);
        return Ok(new { success = true, data = shipment });
    }

    /// <summary>
    /// Obtém recebimentos agendados
    /// </summary>
    [HttpGet("scheduled")]
    public async Task<ActionResult<object>> GetScheduled()
    {
        var shipments = await _service.GetScheduledAsync();
        return Ok(new { success = true, data = shipments });
    }

    /// <summary>
    /// Obtém recebimentos em andamento
    /// </summary>
    [HttpGet("in-progress")]
    public async Task<ActionResult<object>> GetInProgress()
    {
        var shipments = await _service.GetInProgressAsync();
        return Ok(new { success = true, data = shipments });
    }

    /// <summary>
    /// Obtém recebimentos por status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<object>> GetByStatus(int status)
    {
        var shipments = await _service.GetByStatusAsync((InboundStatus)status);
        return Ok(new { success = true, data = shipments });
    }

    /// <summary>
    /// Obtém recebimentos por empresa
    /// </summary>
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<object>> GetByCompany(Guid companyId)
    {
        var shipments = await _service.GetByCompanyIdAsync(companyId);
        return Ok(new { success = true, data = shipments });
    }

    /// <summary>
    /// Cria um novo recebimento
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateInboundShipmentRequest request)
    {
        var shipment = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, new { success = true, data = shipment });
    }

    /// <summary>
    /// Registra o recebimento físico
    /// </summary>
    [HttpPost("{id}/receive")]
    public async Task<ActionResult<object>> Receive(Guid id, [FromBody] ReceiveRequest request)
    {
        await _service.ReceiveAsync(id, request.ReceivedBy);
        return Ok(new { success = true, message = "Recebimento registrado com sucesso" });
    }

    /// <summary>
    /// Completa um recebimento
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<object>> Complete(Guid id)
    {
        await _service.CompleteAsync(id);
        return Ok(new { success = true, message = "Recebimento concluído com sucesso" });
    }

    /// <summary>
    /// Cancela um recebimento
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<object>> Cancel(Guid id)
    {
        await _service.CancelAsync(id);
        return Ok(new { success = true, message = "Recebimento cancelado com sucesso" });
    }

    /// <summary>
    /// Deleta um recebimento
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<object>> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { success = true, message = "Recebimento excluído com sucesso" });
    }
}

public record ReceiveRequest(Guid ReceivedBy);
