using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.StorageLocation;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Logistics.API.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class StorageLocationsController : ControllerBase
{
    private readonly IStorageLocationService _service;
    public StorageLocationsController(IStorageLocationService service) { _service = service; }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StorageLocationResponse>>> Create([FromBody] StorageLocationRequest request)
    {
        try { var response = await _service.CreateAsync(request); return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponse<StorageLocationResponse>.SuccessResponse(response)); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<StorageLocationResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<StorageLocationResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StorageLocationResponse>>> GetById(Guid id)
    {
        try { return Ok(ApiResponse<StorageLocationResponse>.SuccessResponse(await _service.GetByIdAsync(id))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<StorageLocationResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<StorageLocationResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<StorageLocationResponse>>>> GetAll([FromQuery] Guid? warehouseId)
    {
        try { return Ok(ApiResponse<IEnumerable<StorageLocationResponse>>.SuccessResponse(warehouseId.HasValue ? await _service.GetByWarehouseIdAsync(warehouseId.Value) : await _service.GetAllAsync())); }
        catch (Exception) { return StatusCode(500, ApiResponse<IEnumerable<StorageLocationResponse>>.ErrorResponse("Erro interno")); }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<StorageLocationResponse>>> Update(Guid id, [FromBody] StorageLocationRequest request)
    {
        try { return Ok(ApiResponse<StorageLocationResponse>.SuccessResponse(await _service.UpdateAsync(id, request))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<StorageLocationResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<StorageLocationResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try { await _service.DeleteAsync(id); return Ok(ApiResponse<object>.SuccessResponse(null, "Localização deletada")); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno")); }
    }
}
