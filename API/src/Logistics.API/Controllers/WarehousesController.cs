using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Warehouse;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Logistics.API.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _service;
    public WarehousesController(IWarehouseService service) { _service = service; }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<WarehouseResponse>>> Create([FromBody] WarehouseRequest request)
    {
        try { var response = await _service.CreateAsync(request); return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponse<WarehouseResponse>.SuccessResponse(response)); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<WarehouseResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<WarehouseResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<WarehouseResponse>>> GetById(Guid id)
    {
        try { return Ok(ApiResponse<WarehouseResponse>.SuccessResponse(await _service.GetByIdAsync(id))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<WarehouseResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<WarehouseResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IEnumerable<WarehouseResponse>>>> GetAll([FromQuery] Guid? companyId)
    {
        try { return Ok(ApiResponse<IEnumerable<WarehouseResponse>>.SuccessResponse(companyId.HasValue ? await _service.GetByCompanyIdAsync(companyId.Value) : await _service.GetAllAsync())); }
        catch (Exception) { return StatusCode(500, ApiResponse<IEnumerable<WarehouseResponse>>.ErrorResponse("Erro interno")); }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<WarehouseResponse>>> Update(Guid id, [FromBody] WarehouseRequest request)
    {
        try { return Ok(ApiResponse<WarehouseResponse>.SuccessResponse(await _service.UpdateAsync(id, request))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<WarehouseResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<WarehouseResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try { await _service.DeleteAsync(id); return Ok(ApiResponse<object>.SuccessResponse(null, "Armazém deletado")); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno")); }
    }
}
