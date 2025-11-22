using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Inventory;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Logistics.API.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class InventoriesController : ControllerBase
{
    private readonly IInventoryService _service;
    public InventoriesController(IInventoryService service) { _service = service; }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<InventoryResponse>>> Create([FromBody] InventoryRequest request)
    {
        try { var response = await _service.CreateAsync(request); return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponse<InventoryResponse>.SuccessResponse(response)); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<InventoryResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<InventoryResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InventoryResponse>>> GetById(Guid id)
    {
        try { return Ok(ApiResponse<InventoryResponse>.SuccessResponse(await _service.GetByIdAsync(id))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<InventoryResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<InventoryResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InventoryResponse>>>> GetAll([FromQuery] Guid? warehouseId, [FromQuery] Guid? productId)
    {
        try 
        { 
            IEnumerable<InventoryResponse> response;
            if (warehouseId.HasValue) response = await _service.GetByWarehouseIdAsync(warehouseId.Value);
            else if (productId.HasValue) response = await _service.GetByProductIdAsync(productId.Value);
            else response = await _service.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<InventoryResponse>>.SuccessResponse(response)); 
        }
        catch (Exception) { return StatusCode(500, ApiResponse<IEnumerable<InventoryResponse>>.ErrorResponse("Erro interno")); }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<InventoryResponse>>> Update(Guid id, [FromBody] InventoryRequest request)
    {
        try { return Ok(ApiResponse<InventoryResponse>.SuccessResponse(await _service.UpdateAsync(id, request))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<InventoryResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<InventoryResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try { await _service.DeleteAsync(id); return Ok(ApiResponse<object>.SuccessResponse(null, "Inventário deletado")); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno")); }
    }
}
