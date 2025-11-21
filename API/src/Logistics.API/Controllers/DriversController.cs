using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Driver;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DriversController : ControllerBase
{
    private readonly IDriverService _driverService;
    private readonly ILogger<DriversController> _logger;

    public DriversController(IDriverService driverService, ILogger<DriversController> logger)
    {
        _driverService = driverService;
        _logger = logger;
    }

    /// <summary>
    /// Criar novo motorista
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<DriverResponse>>> Create([FromBody] DriverRequest request)
    {
        try
        {
            var response = await _driverService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, 
                ApiResponse<DriverResponse>.SuccessResponse(response, "Motorista criado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<DriverResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<DriverResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar motorista");
            return StatusCode(500, ApiResponse<DriverResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter motorista por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DriverResponse>>> GetById(Guid id)
    {
        try
        {
            var response = await _driverService.GetByIdAsync(id);
            return Ok(ApiResponse<DriverResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<DriverResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar motorista");
            return StatusCode(500, ApiResponse<DriverResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Listar todos os motoristas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<DriverResponse>>>> GetAll([FromQuery] Guid? companyId)
    {
        try
        {
            IEnumerable<DriverResponse> response;

            if (companyId.HasValue)
            {
                response = await _driverService.GetByCompanyIdAsync(companyId.Value);
            }
            else
            {
                response = await _driverService.GetAllAsync();
            }

            return Ok(ApiResponse<IEnumerable<DriverResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar motoristas");
            return StatusCode(500, ApiResponse<IEnumerable<DriverResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar motorista
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<DriverResponse>>> Update(Guid id, [FromBody] DriverRequest request)
    {
        try
        {
            var response = await _driverService.UpdateAsync(id, request);
            return Ok(ApiResponse<DriverResponse>.SuccessResponse(response, "Motorista atualizado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<DriverResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<DriverResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar motorista");
            return StatusCode(500, ApiResponse<DriverResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Ativar motorista
    /// </summary>
    [HttpPatch("{id}/activate")]
    public async Task<ActionResult<ApiResponse<object>>> Activate(Guid id)
    {
        try
        {
            await _driverService.ActivateAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Motorista ativado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ativar motorista");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Desativar motorista
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult<ApiResponse<object>>> Deactivate(Guid id)
    {
        try
        {
            await _driverService.DeactivateAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Motorista desativado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar motorista");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Deletar motorista
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _driverService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Motorista deletado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar motorista");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }
}
