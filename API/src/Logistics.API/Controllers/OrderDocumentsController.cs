using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Logistics.API.Controllers;

public record SoftDeleteRequest(Guid DeletedBy);

[ApiController]
[Route("api/orders/{orderId}/documents")]
[Authorize]
public class OrderDocumentsController : ControllerBase
{
    private readonly IOrderDocumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderDocumentsController(IOrderDocumentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult> GetByOrder(Guid orderId)
    {
        var documents = await _repository.GetByOrderIdAsync(orderId);
        return Ok(documents);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid orderId, Guid id)
    {
        var document = await _repository.GetByIdAsync(id);
        if (document == null || document.OrderId != orderId || document.DeletedAt != null)
            return NotFound();
        return Ok(document);
    }

    [HttpPost("{id}/soft-delete")]
    public async Task<ActionResult> SoftDelete(Guid orderId, Guid id, [FromBody] SoftDeleteRequest request)
    {
        var document = await _repository.GetByIdAsync(id);
        if (document == null || document.OrderId != orderId || document.DeletedAt != null)
            return NotFound();

        document.SoftDelete(request.DeletedBy);
        await _unitOfWork.CommitAsync();
        return Ok(new { message = "Documento deletado", deletedAt = document.DeletedAt, deletedBy = document.DeletedBy });
    }
}
