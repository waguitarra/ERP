using Logistics.Application.DTOs.Order;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var order = await _service.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<List<OrderResponse>>> GetByCompany(Guid companyId)
    {
        var orders = await _service.GetByCompanyIdAsync(companyId);
        return Ok(orders.ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }
}
