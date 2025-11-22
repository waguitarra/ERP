using Logistics.Application.DTOs.PackingTask;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackingTasksController : ControllerBase
{
    private readonly IPackingTaskService _service;

    public PackingTasksController(IPackingTaskService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<PackingTaskResponse>> Create([FromBody] CreatePackingTaskRequest request)
    {
        var task = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PackingTaskResponse>> GetById(Guid id)
    {
        var task = await _service.GetByIdAsync(id);
        return Ok(task);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<List<PackingTaskResponse>>> GetByOrder(Guid orderId)
    {
        var tasks = await _service.GetByOrderIdAsync(orderId);
        return Ok(tasks.ToList());
    }

    [HttpPost("{id}/start")]
    public async Task<ActionResult> Start(Guid id)
    {
        await _service.StartTaskAsync(id);
        return Ok();
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult> Complete(Guid id)
    {
        await _service.CompleteTaskAsync(id);
        return Ok();
    }
}
