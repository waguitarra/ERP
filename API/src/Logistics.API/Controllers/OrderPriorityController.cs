using Logistics.Application.DTOs.OrderPriority;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderPriorityController : ControllerBase
{
    private readonly IOrderPriorityService _service;

    public OrderPriorityController(IOrderPriorityService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todas as prioridades de pedidos com tradução
    /// </summary>
    /// <param name="language">Idioma (pt, en, es)</param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderPriorityResponse>>> GetAll([FromQuery] string language = "pt")
    {
        var priorities = await _service.GetAllAsync(language);
        return Ok(priorities);
    }

    /// <summary>
    /// Busca prioridade por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderPriorityResponse>> GetById(int id, [FromQuery] string language = "pt")
    {
        var priority = await _service.GetByIdAsync(id, language);
        if (priority == null)
            return NotFound();
        return Ok(priority);
    }

    /// <summary>
    /// Busca prioridade por código
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<OrderPriorityResponse>> GetByCode(string code, [FromQuery] string language = "pt")
    {
        var priority = await _service.GetByCodeAsync(code, language);
        if (priority == null)
            return NotFound();
        return Ok(priority);
    }
}
