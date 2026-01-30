using Logistics.Application.DTOs.PickingTask;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/picking-tasks")]
[Authorize]
public class PickingTasksController : ControllerBase
{
    private readonly IPickingTaskService _service;

    public PickingTasksController(IPickingTaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PickingTaskResponse>>> GetAll()
    {
        Log.Information("[PickingTasksController] GET /api/picking-tasks");
        var tasks = await _service.GetAllAsync();
        return Ok(new { success = true, data = tasks });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PickingTaskResponse>> GetById(Guid id)
    {
        Log.Information("[PickingTasksController] GET /api/picking-tasks/{Id}", id);
        try
        {
            var task = await _service.GetByIdAsync(id);
            return Ok(new { success = true, data = task });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { success = false, message = "Picking Task não encontrada" });
        }
    }

    [HttpGet("wave/{waveId}")]
    public async Task<ActionResult<IEnumerable<PickingTaskResponse>>> GetByWave(Guid waveId)
    {
        Log.Information("[PickingTasksController] GET /api/picking-tasks/wave/{WaveId}", waveId);
        var tasks = await _service.GetByWaveIdAsync(waveId);
        return Ok(new { success = true, data = tasks });
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<PickingTaskResponse>>> GetByStatus(int status)
    {
        Log.Information("[PickingTasksController] GET /api/picking-tasks/status/{Status}", status);
        var tasks = await _service.GetByStatusAsync(status);
        return Ok(new { success = true, data = tasks });
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<PickingTaskResponse>>> GetPending()
    {
        Log.Information("[PickingTasksController] GET /api/picking-tasks/pending");
        var tasks = await _service.GetPendingAsync();
        return Ok(new { success = true, data = tasks });
    }

    [HttpGet("in-progress")]
    public async Task<ActionResult<IEnumerable<PickingTaskResponse>>> GetInProgress()
    {
        Log.Information("[PickingTasksController] GET /api/picking-tasks/in-progress");
        var tasks = await _service.GetInProgressAsync();
        return Ok(new { success = true, data = tasks });
    }

    [HttpPost("{id}/assign")]
    public async Task<ActionResult> Assign(Guid id, [FromBody] AssignRequest request)
    {
        Log.Information("[PickingTasksController] POST /api/picking-tasks/{Id}/assign", id);
        try
        {
            await _service.AssignTaskAsync(id, request.UserId);
            return Ok(new { success = true, message = "Task atribuída com sucesso" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { success = false, message = "Picking Task não encontrada" });
        }
    }

    [HttpPost("{id}/start")]
    public async Task<ActionResult> Start(Guid id)
    {
        Log.Information("[PickingTasksController] POST /api/picking-tasks/{Id}/start", id);
        try
        {
            await _service.StartTaskAsync(id);
            return Ok(new { success = true, message = "Task iniciada com sucesso" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { success = false, message = "Picking Task não encontrada" });
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult> Complete(Guid id)
    {
        Log.Information("[PickingTasksController] POST /api/picking-tasks/{Id}/complete", id);
        try
        {
            await _service.CompleteTaskAsync(id);
            return Ok(new { success = true, message = "Task completada com sucesso" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { success = false, message = "Picking Task não encontrada" });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> Cancel(Guid id)
    {
        Log.Information("[PickingTasksController] POST /api/picking-tasks/{Id}/cancel", id);
        try
        {
            await _service.CancelTaskAsync(id);
            return Ok(new { success = true, message = "Task cancelada com sucesso" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { success = false, message = "Picking Task não encontrada" });
        }
    }

    [HttpPost("{id}/lines/{lineId}/pick")]
    public async Task<ActionResult> PickLine(Guid id, Guid lineId, [FromBody] PickLineRequest request)
    {
        Log.Information("[PickingTasksController] POST /api/picking-tasks/{Id}/lines/{LineId}/pick", id, lineId);
        try
        {
            await _service.PickLineAsync(id, lineId, request.QuantityPicked);
            return Ok(new { success = true, message = "Linha coletada com sucesso" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { success = false, message = "Picking Line não encontrada" });
        }
    }
}

public class AssignRequest
{
    public Guid UserId { get; set; }
}

public class PickLineRequest
{
    public decimal QuantityPicked { get; set; }
}
