using Logistics.Application.DTOs.Receipt;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _service;

    public ReceiptsController(IReceiptService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ReceiptResponse>> Create(CreateReceiptRequest request)
    {
        var receipt = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = receipt.Id }, receipt);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReceiptResponse>> GetById(Guid id)
    {
        var receipt = await _service.GetByIdAsync(id);
        return Ok(receipt);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<List<ReceiptResponse>>> GetByWarehouse(Guid warehouseId)
    {
        var receipts = await _service.GetByWarehouseIdAsync(warehouseId);
        return Ok(receipts.ToList());
    }

    [HttpGet]
    public async Task<ActionResult<List<ReceiptResponse>>> GetAll()
    {
        var receipts = await _service.GetAllAsync();
        return Ok(receipts.ToList());
    }
}
