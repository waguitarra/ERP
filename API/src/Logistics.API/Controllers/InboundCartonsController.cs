using Logistics.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/inbound-cartons")]
[Authorize]
public class InboundCartonsController : ControllerBase
{
    private readonly IInboundCartonRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public InboundCartonsController(IInboundCartonRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult> GetByBarcode(string barcode)
    {
        var carton = await _repository.GetByBarcodeAsync(barcode);
        if (carton == null)
            return NotFound(new { message = $"Carton com barcode '{barcode}' não encontrado" });
        return Ok(carton);
    }

    [HttpGet("parcel/{parcelId}")]
    public async Task<ActionResult> GetByParcel(Guid parcelId)
    {
        var cartons = await _repository.GetByParcelIdAsync(parcelId);
        return Ok(cartons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var carton = await _repository.GetByIdAsync(id);
        if (carton == null)
            return NotFound();
        return Ok(carton);
    }

    [HttpPost("{id}/receive")]
    public async Task<ActionResult> MarkAsReceived(Guid id, [FromBody] ReceiveCartonRequest request)
    {
        var carton = await _repository.GetByIdAsync(id);
        if (carton == null)
            return NotFound();

        carton.MarkAsReceived(request.ReceivedBy);
        await _unitOfWork.CommitAsync();
        return Ok(carton);
    }
}

public record ReceiveCartonRequest(Guid ReceivedBy);
