using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Product;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService service, ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Create([FromBody] ProductRequest request)
    {
        try
        {
            var response = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, 
                ApiResponse<ProductResponse>.SuccessResponse(response, "Produto criado com sucesso"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<ProductResponse>.ErrorResponse(ex.Message)); }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse<ProductResponse>.ErrorResponse(ex.Message)); }
        catch (Exception ex) { _logger.LogError(ex, "Erro ao criar produto"); return StatusCode(500, ApiResponse<ProductResponse>.ErrorResponse("Erro interno")); }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> GetById(Guid id)
    {
        try
        {
            var response = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<ProductResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<ProductResponse>.ErrorResponse(ex.Message)); }
        catch (Exception ex) { _logger.LogError(ex, "Erro"); return StatusCode(500, ApiResponse<ProductResponse>.ErrorResponse("Erro interno")); }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetAll([FromQuery] Guid? companyId)
    {
        try
        {
            var response = companyId.HasValue ? 
                await _service.GetByCompanyIdAsync(companyId.Value) : 
                await _service.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(response));
        }
        catch (Exception ex) { _logger.LogError(ex, "Erro"); return StatusCode(500, ApiResponse<IEnumerable<ProductResponse>>.ErrorResponse("Erro interno")); }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Update(Guid id, [FromBody] ProductRequest request)
    {
        try
        {
            var response = await _service.UpdateAsync(id, request);
            return Ok(ApiResponse<ProductResponse>.SuccessResponse(response, "Produto atualizado"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<ProductResponse>.ErrorResponse(ex.Message)); }
        catch (Exception ex) { _logger.LogError(ex, "Erro"); return StatusCode(500, ApiResponse<ProductResponse>.ErrorResponse("Erro interno")); }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Produto deletado"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.ErrorResponse(ex.Message)); }
        catch (Exception ex) { _logger.LogError(ex, "Erro"); return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno")); }
    }
}
