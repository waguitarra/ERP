using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Customer;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Logistics.API.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    public CustomersController(ICustomerService service) { _service = service; }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> Create([FromBody] CustomerRequest request)
    {
        try { var response = await _service.CreateAsync(request); return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponse<CustomerResponse>.SuccessResponse(response)); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<CustomerResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<CustomerResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> GetById(Guid id)
    {
        try { return Ok(ApiResponse<CustomerResponse>.SuccessResponse(await _service.GetByIdAsync(id))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<CustomerResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<CustomerResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerResponse>>>> GetAll([FromQuery] Guid? companyId)
    {
        try { return Ok(ApiResponse<IEnumerable<CustomerResponse>>.SuccessResponse(companyId.HasValue ? await _service.GetByCompanyIdAsync(companyId.Value) : await _service.GetAllAsync())); }
        catch (Exception) { return StatusCode(500, ApiResponse<IEnumerable<CustomerResponse>>.ErrorResponse("Erro interno")); }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> Update(Guid id, [FromBody] CustomerRequest request)
    {
        try { return Ok(ApiResponse<CustomerResponse>.SuccessResponse(await _service.UpdateAsync(id, request))); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<CustomerResponse>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<CustomerResponse>.ErrorResponse("Erro interno")); }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try { await _service.DeleteAsync(id); return Ok(ApiResponse<object>.SuccessResponse(null, "Cliente deletado")); }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.ErrorResponse(ex.Message)); }
        catch (Exception) { return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno")); }
    }
}
