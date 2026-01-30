using Logistics.Application.DTOs.PackingTask;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/packing-tasks")]
[Authorize]
public class PackingTasksController : ControllerBase
{
    private readonly IPackingTaskService _service;

    public PackingTasksController(IPackingTaskService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtém todas as tarefas de empacotamento
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetAll()
    {
        var tasks = await _service.GetAllAsync();
        return Ok(new { success = true, data = tasks });
    }

    /// <summary>
    /// Obtém uma tarefa por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(Guid id)
    {
        var task = await _service.GetByIdAsync(id);
        return Ok(new { success = true, data = task });
    }

    /// <summary>
    /// Obtém tarefas pendentes
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<object>> GetPending()
    {
        var tasks = await _service.GetPendingAsync();
        return Ok(new { success = true, data = tasks });
    }

    /// <summary>
    /// Obtém tarefas em andamento
    /// </summary>
    [HttpGet("in-progress")]
    public async Task<ActionResult<object>> GetInProgress()
    {
        var tasks = await _service.GetInProgressAsync();
        return Ok(new { success = true, data = tasks });
    }

    /// <summary>
    /// Obtém tarefas por status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<object>> GetByStatus(int status)
    {
        var tasks = await _service.GetByStatusAsync((WMSTaskStatus)status);
        return Ok(new { success = true, data = tasks });
    }

    /// <summary>
    /// Obtém tarefas por pedido
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<object>> GetByOrder(Guid orderId)
    {
        var tasks = await _service.GetByOrderIdAsync(orderId);
        return Ok(new { success = true, data = tasks });
    }

    /// <summary>
    /// Cria uma nova tarefa de empacotamento
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreatePackingTaskRequest request)
    {
        var task = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, new { success = true, data = task });
    }

    /// <summary>
    /// Inicia uma tarefa
    /// </summary>
    [HttpPost("{id}/start")]
    public async Task<ActionResult<object>> Start(Guid id)
    {
        await _service.StartTaskAsync(id);
        return Ok(new { success = true, message = "Tarefa iniciada com sucesso" });
    }

    /// <summary>
    /// Completa uma tarefa
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<object>> Complete(Guid id)
    {
        await _service.CompleteTaskAsync(id);
        return Ok(new { success = true, message = "Tarefa concluída com sucesso" });
    }

    /// <summary>
    /// Cancela uma tarefa
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<object>> Cancel(Guid id)
    {
        await _service.CancelTaskAsync(id);
        return Ok(new { success = true, message = "Tarefa cancelada com sucesso" });
    }

    /// <summary>
    /// Deleta uma tarefa
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<object>> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { success = true, message = "Tarefa excluída com sucesso" });
    }
}
