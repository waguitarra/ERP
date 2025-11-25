using Logistics.Application.DTOs.OrderStatus;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderStatusController : ControllerBase
{
    private readonly IOrderStatusService _service;

    public OrderStatusController(IOrderStatusService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todos os status de pedidos com tradução
    /// </summary>
    /// <param name="language">Idioma (pt, en, es)</param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderStatusResponse>>> GetAll([FromQuery] string language = "pt")
    {
        var statuses = await _service.GetAllAsync(language);
        return Ok(statuses);
    }

    /// <summary>
    /// Busca status por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderStatusResponse>> GetById(int id, [FromQuery] string language = "pt")
    {
        var status = await _service.GetByIdAsync(id, language);
        if (status == null)
            return NotFound();
        return Ok(status);
    }

    /// <summary>
    /// Busca status por código
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<OrderStatusResponse>> GetByCode(string code, [FromQuery] string language = "pt")
    {
        var status = await _service.GetByCodeAsync(code, language);
        if (status == null)
            return NotFound();
        return Ok(status);
    }
}
