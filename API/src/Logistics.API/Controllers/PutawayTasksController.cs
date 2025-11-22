using Logistics.Application.DTOs.PutawayTask;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PutawayTasksController : ControllerBase
{
    private readonly IPutawayTaskService _service;

    public PutawayTasksController(IPutawayTaskService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<PutawayTaskResponse>> Create([FromBody] CreatePutawayTaskRequest request)
    {
        var task = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PutawayTaskResponse>> GetById(Guid id)
    {
        var task = await _service.GetByIdAsync(id);
        return Ok(task);
    }

    [HttpGet("receipt/{receiptId}")]
    public async Task<ActionResult<List<PutawayTaskResponse>>> GetByReceipt(Guid receiptId)
    {
        var tasks = await _service.GetByReceiptIdAsync(receiptId);
        return Ok(tasks.ToList());
    }

    [HttpPost("{id}/assign")]
    public async Task<ActionResult> Assign(Guid id, [FromBody] Guid userId)
    {
        await _service.AssignTaskAsync(id, userId);
        return Ok();
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
