using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.StockMovement;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Logistics.API.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class StockMovementsController : ControllerBase
{
    private readonly IStockMovementService _service;
    public StockMovementsController(IStockMovementService service) { _service = service; }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StockMovementResponse>>> Create([FromBody] StockMovementRequest request)
    {
        try { var response = await _service.CreateAsync(request); return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponse<StockMovementResponse>.SuccessResponse(response)); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<StockMovementResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<StockMovementResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StockMovementResponse>>> GetById(Guid id)
    {
        try { return Ok(ApiResponse<StockMovementResponse>.SuccessResponse(await _service.GetByIdAsync(id))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<StockMovementResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<StockMovementResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockMovementResponse>>>> GetAll([FromQuery] Guid? warehouseId, [FromQuery] Guid? productId)
    {
        try 
        { 
            IEnumerable<StockMovementResponse> response;
            if (warehouseId.HasValue) response = await _service.GetByWarehouseIdAsync(warehouseId.Value);
            else if (productId.HasValue) response = await _service.GetByProductIdAsync(productId.Value);
            else response = await _service.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<StockMovementResponse>>.SuccessResponse(response)); 
        }
        catch (Exception) { return StatusCode(500, ApiResponse<IEnumerable<StockMovementResponse>>.ErrorResponse("Erro interno")); }
    }
}
