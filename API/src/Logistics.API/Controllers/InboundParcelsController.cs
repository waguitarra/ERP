using Logistics.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/inbound-parcels")]
[Authorize]
public class InboundParcelsController : ControllerBase
{
    private readonly IInboundParcelRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public InboundParcelsController(IInboundParcelRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("lpn/{lpn}")]
    public async Task<ActionResult> GetByLPN(string lpn)
    {
        var parcel = await _repository.GetByLPNAsync(lpn);
        if (parcel == null)
            return NotFound(new { message = $"Parcel com LPN '{lpn}' não encontrado" });
        return Ok(parcel);
    }

    [HttpGet("shipment/{shipmentId}")]
    public async Task<ActionResult> GetByShipment(Guid shipmentId)
    {
        var parcels = await _repository.GetByShipmentIdAsync(shipmentId);
        return Ok(parcels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var parcel = await _repository.GetByIdAsync(id);
        if (parcel == null)
            return NotFound();
        return Ok(parcel);
    }

    [HttpPost("{id}/receive")]
    public async Task<ActionResult> MarkAsReceived(Guid id, [FromBody] ReceiveParcelRequest request)
    {
        var parcel = await _repository.GetByIdAsync(id);
        if (parcel == null)
            return NotFound();

        parcel.MarkAsReceived(request.ReceivedBy, request.Location);
        await _unitOfWork.CommitAsync();
        return Ok(parcel);
    }
}

public record ReceiveParcelRequest(Guid ReceivedBy, string? Location);
