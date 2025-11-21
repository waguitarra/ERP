using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Supplier;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Logistics.API.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _service;
    public SuppliersController(ISupplierService service) { _service = service; }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<SupplierResponse>>> Create([FromBody] SupplierRequest request)
    {
        try { var response = await _service.CreateAsync(request); return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponse<SupplierResponse>.SuccessResponse(response)); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<SupplierResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<SupplierResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SupplierResponse>>> GetById(Guid id)
    {
        try { return Ok(ApiResponse<SupplierResponse>.SuccessResponse(await _service.GetByIdAsync(id))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<SupplierResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<SupplierResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SupplierResponse>>>> GetAll([FromQuery] Guid? companyId)
    {
        try { return Ok(ApiResponse<IEnumerable<SupplierResponse>>.SuccessResponse(companyId.HasValue ? await _service.GetByCompanyIdAsync(companyId.Value) : await _service.GetAllAsync())); }
        catch (Exception) { return StatusCode(500, ApiResponse<IEnumerable<SupplierResponse>>.ErrorResponse("Erro interno")); }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<SupplierResponse>>> Update(Guid id, [FromBody] SupplierRequest request)
    {
        try { return Ok(ApiResponse<SupplierResponse>.SuccessResponse(await _service.UpdateAsync(id, request))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<SupplierResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<SupplierResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try { await _service.DeleteAsync(id); return Ok(ApiResponse<object>.SuccessResponse(null, "Fornecedor deletado")); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno")); }
    }
}
