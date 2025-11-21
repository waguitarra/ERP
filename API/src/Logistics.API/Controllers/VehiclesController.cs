using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Vehicle;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IVehicleService vehicleService, ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    /// <summary>
    /// Criar novo veículo
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> Create([FromBody] VehicleRequest request)
    {
        try
        {
            var response = await _vehicleService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, 
                ApiResponse<VehicleResponse>.SuccessResponse(response, "Veículo criado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar veículo");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter veículo por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> GetById(Guid id)
    {
        try
        {
            var response = await _vehicleService.GetByIdAsync(id);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar veículo");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Listar todos os veículos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleResponse>>>> GetAll([FromQuery] Guid? companyId)
    {
        try
        {
            IEnumerable<VehicleResponse> response;

            if (companyId.HasValue)
            {
                response = await _vehicleService.GetByCompanyIdAsync(companyId.Value);
            }
            else
            {
                response = await _vehicleService.GetAllAsync();
            }

            return Ok(ApiResponse<IEnumerable<VehicleResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar veículos");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar veículo
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> Update(Guid id, [FromBody] VehicleRequest request)
    {
        try
        {
            var response = await _vehicleService.UpdateAsync(id, request);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response, "Veículo atualizado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar veículo");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar status do veículo
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var response = await _vehicleService.UpdateStatusAsync(id, request.Status);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response, "Status atualizado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do veículo");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Deletar veículo
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _vehicleService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Veículo deletado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar veículo");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
