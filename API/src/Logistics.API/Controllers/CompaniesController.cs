using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Company;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    /// <summary>
    /// Criar nova empresa (Admin Only)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> Create([FromBody] CompanyRequest request)
    {
        try
        {
            var response = await _companyService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, 
                ApiResponse<CompanyResponse>.SuccessResponse(response, "Empresa criada com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CompanyResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar empresa");
            return StatusCode(500, ApiResponse<CompanyResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter empresa por ID (Admin Only)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> GetById(Guid id)
    {
        try
        {
            var response = await _companyService.GetByIdAsync(id);
            return Ok(ApiResponse<CompanyResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CompanyResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar empresa");
            return StatusCode(500, ApiResponse<CompanyResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Listar todas as empresas (Admin Only)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompanyResponse>>>> GetAll()
    {
        try
        {
            var response = await _companyService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CompanyResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar empresas");
            return StatusCode(500, ApiResponse<IEnumerable<CompanyResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar empresa (Admin Only)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> Update(Guid id, [FromBody] CompanyRequest request)
    {
        try
        {
            var response = await _companyService.UpdateAsync(id, request);
            return Ok(ApiResponse<CompanyResponse>.SuccessResponse(response, "Empresa atualizada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CompanyResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CompanyResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar empresa");
            return StatusCode(500, ApiResponse<CompanyResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Desativar empresa (Admin Only)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _companyService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Empresa desativada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar empresa");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }
}
