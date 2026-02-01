using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.Vehicle;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly IVehicleManagementService _vehicleManagementService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(
        IVehicleService vehicleService, 
        IVehicleManagementService vehicleManagementService,
        ILogger<VehiclesController> logger)
    {
        _vehicleService = vehicleService;
        _vehicleManagementService = vehicleManagementService;
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

    /// <summary>
    /// Habilitar rastreamento do veículo
    /// </summary>
    [HttpPost("{id}/tracking/enable")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> EnableTracking(Guid id)
    {
        try
        {
            var response = await _vehicleService.EnableTrackingAsync(id);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response, "Rastreamento habilitado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao habilitar rastreamento");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Desabilitar rastreamento do veículo
    /// </summary>
    [HttpPost("{id}/tracking/disable")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> DisableTracking(Guid id)
    {
        try
        {
            var response = await _vehicleService.DisableTrackingAsync(id);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response, "Rastreamento desabilitado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desabilitar rastreamento");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Regenerar token de rastreamento
    /// </summary>
    [HttpPost("{id}/tracking/regenerate-token")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> RegenerateTrackingToken(Guid id)
    {
        try
        {
            var response = await _vehicleService.RegenerateTrackingTokenAsync(id);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response, "Token de rastreamento regenerado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao regenerar token de rastreamento");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar localização do veículo
    /// </summary>
    [HttpPost("{id}/location")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> UpdateLocation(Guid id, [FromBody] UpdateVehicleLocationRequest request)
    {
        try
        {
            var response = await _vehicleService.UpdateLocationAsync(id, request);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response, "Localização atualizada com sucesso"));
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
            _logger.LogError(ex, "Erro ao atualizar localização");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Listar veículos com rastreamento habilitado
    /// </summary>
    [HttpGet("tracking/enabled")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleResponse>>>> GetWithTrackingEnabled()
    {
        try
        {
            var response = await _vehicleService.GetWithTrackingEnabledAsync();
            return Ok(ApiResponse<IEnumerable<VehicleResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar veículos com rastreamento");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atribuir motorista ao veículo
    /// </summary>
    [HttpPost("{id}/driver")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> AssignDriver(Guid id, [FromBody] AssignDriverRequest request)
    {
        try
        {
            var response = await _vehicleService.AssignDriverAsync(id, request);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response, "Motorista atribuído com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atribuir motorista");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Remover motorista do veículo
    /// </summary>
    [HttpDelete("{id}/driver")]
    public async Task<ActionResult<ApiResponse<VehicleResponse>>> RemoveDriver(Guid id)
    {
        try
        {
            var response = await _vehicleService.RemoveDriverAsync(id);
            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(response, "Motorista removido com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover motorista");
            return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter resumo de gestão do veículo (custos, manutenções, alertas)
    /// </summary>
    [HttpGet("{id}/management/summary")]
    public async Task<ActionResult<ApiResponse<VehicleSummaryResponse>>> GetVehicleSummary(Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetVehicleSummaryAsync(id);
            return Ok(ApiResponse<VehicleSummaryResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleSummaryResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter resumo do veículo");
            return StatusCode(500, ApiResponse<VehicleSummaryResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter alertas do veículo (inspeções/documentos vencendo)
    /// </summary>
    [HttpGet("{id}/management/alerts")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleAlertResponse>>>> GetVehicleAlerts(Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetAlertsByVehicleAsync(id);
            return Ok(ApiResponse<IEnumerable<VehicleAlertResponse>>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<IEnumerable<VehicleAlertResponse>>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter alertas do veículo");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleAlertResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    // ============ MAINTENANCES ============

    /// <summary>
    /// Listar manutenções do veículo
    /// </summary>
    [HttpGet("{id}/management/maintenances")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleMaintenanceResponse>>>> GetMaintenances(Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetMaintenancesByVehicleAsync(id);
            return Ok(ApiResponse<IEnumerable<VehicleMaintenanceResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar manutenções do veículo");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleMaintenanceResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter manutenção por ID
    /// </summary>
    [HttpGet("{vehicleId}/management/maintenances/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleMaintenanceResponse>>> GetMaintenance(Guid vehicleId, Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetMaintenanceByIdAsync(id);
            return Ok(ApiResponse<VehicleMaintenanceResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleMaintenanceResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter manutenção");
            return StatusCode(500, ApiResponse<VehicleMaintenanceResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Criar manutenção do veículo
    /// </summary>
    [HttpPost("{id}/management/maintenances")]
    public async Task<ActionResult<ApiResponse<VehicleMaintenanceResponse>>> CreateMaintenance(Guid id, [FromBody] CreateVehicleMaintenanceRequest request)
    {
        try
        {
            request.VehicleId = id;
            var response = await _vehicleManagementService.CreateMaintenanceAsync(request);
            return CreatedAtAction(nameof(GetMaintenance), new { vehicleId = id, id = response.Id }, 
                ApiResponse<VehicleMaintenanceResponse>.SuccessResponse(response, "Manutenção criada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleMaintenanceResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar manutenção");
            return StatusCode(500, ApiResponse<VehicleMaintenanceResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar manutenção
    /// </summary>
    [HttpPut("{vehicleId}/management/maintenances/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleMaintenanceResponse>>> UpdateMaintenance(Guid vehicleId, Guid id, [FromBody] UpdateVehicleMaintenanceRequest request)
    {
        try
        {
            var response = await _vehicleManagementService.UpdateMaintenanceAsync(id, request);
            return Ok(ApiResponse<VehicleMaintenanceResponse>.SuccessResponse(response, "Manutenção atualizada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleMaintenanceResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar manutenção");
            return StatusCode(500, ApiResponse<VehicleMaintenanceResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Deletar manutenção
    /// </summary>
    [HttpDelete("{vehicleId}/management/maintenances/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMaintenance(Guid vehicleId, Guid id)
    {
        try
        {
            await _vehicleManagementService.DeleteMaintenanceAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Manutenção deletada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar manutenção");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }

    // ============ INSPECTIONS ============

    /// <summary>
    /// Listar inspeções do veículo
    /// </summary>
    [HttpGet("{id}/management/inspections")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleInspectionResponse>>>> GetInspections(Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetInspectionsByVehicleAsync(id);
            return Ok(ApiResponse<IEnumerable<VehicleInspectionResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar inspeções do veículo");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleInspectionResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter inspeção por ID
    /// </summary>
    [HttpGet("{vehicleId}/management/inspections/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleInspectionResponse>>> GetInspection(Guid vehicleId, Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetInspectionByIdAsync(id);
            return Ok(ApiResponse<VehicleInspectionResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleInspectionResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter inspeção");
            return StatusCode(500, ApiResponse<VehicleInspectionResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Criar inspeção do veículo
    /// </summary>
    [HttpPost("{id}/management/inspections")]
    public async Task<ActionResult<ApiResponse<VehicleInspectionResponse>>> CreateInspection(Guid id, [FromBody] CreateVehicleInspectionRequest request)
    {
        try
        {
            request.VehicleId = id;
            var response = await _vehicleManagementService.CreateInspectionAsync(request);
            return CreatedAtAction(nameof(GetInspection), new { vehicleId = id, id = response.Id }, 
                ApiResponse<VehicleInspectionResponse>.SuccessResponse(response, "Inspeção criada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleInspectionResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar inspeção");
            return StatusCode(500, ApiResponse<VehicleInspectionResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar inspeção
    /// </summary>
    [HttpPut("{vehicleId}/management/inspections/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleInspectionResponse>>> UpdateInspection(Guid vehicleId, Guid id, [FromBody] UpdateVehicleInspectionRequest request)
    {
        try
        {
            var response = await _vehicleManagementService.UpdateInspectionAsync(id, request);
            return Ok(ApiResponse<VehicleInspectionResponse>.SuccessResponse(response, "Inspeção atualizada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleInspectionResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar inspeção");
            return StatusCode(500, ApiResponse<VehicleInspectionResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Deletar inspeção
    /// </summary>
    [HttpDelete("{vehicleId}/management/inspections/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteInspection(Guid vehicleId, Guid id)
    {
        try
        {
            await _vehicleManagementService.DeleteInspectionAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Inspeção deletada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar inspeção");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }

    // ============ DOCUMENTS ============

    /// <summary>
    /// Listar documentos do veículo
    /// </summary>
    [HttpGet("{id}/management/documents")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleDocumentResponse>>>> GetDocuments(Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetDocumentsByVehicleAsync(id);
            return Ok(ApiResponse<IEnumerable<VehicleDocumentResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar documentos do veículo");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleDocumentResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter documento por ID
    /// </summary>
    [HttpGet("{vehicleId}/management/documents/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleDocumentResponse>>> GetDocument(Guid vehicleId, Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetDocumentByIdAsync(id);
            return Ok(ApiResponse<VehicleDocumentResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleDocumentResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter documento");
            return StatusCode(500, ApiResponse<VehicleDocumentResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Criar documento do veículo
    /// </summary>
    [HttpPost("{id}/management/documents")]
    public async Task<ActionResult<ApiResponse<VehicleDocumentResponse>>> CreateDocument(Guid id, [FromBody] CreateVehicleDocumentRequest request)
    {
        try
        {
            request.VehicleId = id;
            var response = await _vehicleManagementService.CreateDocumentAsync(request);
            return CreatedAtAction(nameof(GetDocument), new { vehicleId = id, id = response.Id }, 
                ApiResponse<VehicleDocumentResponse>.SuccessResponse(response, "Documento criado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleDocumentResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar documento");
            return StatusCode(500, ApiResponse<VehicleDocumentResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar documento
    /// </summary>
    [HttpPut("{vehicleId}/management/documents/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleDocumentResponse>>> UpdateDocument(Guid vehicleId, Guid id, [FromBody] UpdateVehicleDocumentRequest request)
    {
        try
        {
            var response = await _vehicleManagementService.UpdateDocumentAsync(id, request);
            return Ok(ApiResponse<VehicleDocumentResponse>.SuccessResponse(response, "Documento atualizado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleDocumentResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar documento");
            return StatusCode(500, ApiResponse<VehicleDocumentResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Deletar documento
    /// </summary>
    [HttpDelete("{vehicleId}/management/documents/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDocument(Guid vehicleId, Guid id)
    {
        try
        {
            await _vehicleManagementService.DeleteDocumentAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Documento deletado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar documento");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }

    // ============ DAMAGES ============

    /// <summary>
    /// Listar avarias do veículo
    /// </summary>
    [HttpGet("{id}/management/damages")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleDamageResponse>>>> GetDamages(Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetDamagesByVehicleAsync(id);
            return Ok(ApiResponse<IEnumerable<VehicleDamageResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar avarias do veículo");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleDamageResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter avaria por ID
    /// </summary>
    [HttpGet("{vehicleId}/management/damages/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleDamageResponse>>> GetDamage(Guid vehicleId, Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetDamageByIdAsync(id);
            return Ok(ApiResponse<VehicleDamageResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleDamageResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter avaria");
            return StatusCode(500, ApiResponse<VehicleDamageResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Listar todas as avarias com vínculo ao veículo
    /// </summary>
    [HttpGet("damages")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleDamageResponse>>>> GetAllDamages([FromQuery] Guid? companyId)
    {
        try
        {
            var response = await _vehicleManagementService.GetAllDamagesAsync(companyId);
            return Ok(ApiResponse<IEnumerable<VehicleDamageResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar avarias");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleDamageResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Criar avaria do veículo
    /// </summary>
    [HttpPost("{id}/management/damages")]
    public async Task<ActionResult<ApiResponse<VehicleDamageResponse>>> CreateDamage(Guid id, [FromBody] CreateVehicleDamageRequest request)
    {
        try
        {
            request.VehicleId = id;
            var response = await _vehicleManagementService.CreateDamageAsync(request);
            return CreatedAtAction(nameof(GetDamage), new { vehicleId = id, id = response.Id }, 
                ApiResponse<VehicleDamageResponse>.SuccessResponse(response, "Avaria criada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleDamageResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar avaria");
            return StatusCode(500, ApiResponse<VehicleDamageResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Atualizar avaria
    /// </summary>
    [HttpPut("{vehicleId}/management/damages/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleDamageResponse>>> UpdateDamage(Guid vehicleId, Guid id, [FromBody] UpdateVehicleDamageRequest request)
    {
        try
        {
            var response = await _vehicleManagementService.UpdateDamageAsync(id, request);
            return Ok(ApiResponse<VehicleDamageResponse>.SuccessResponse(response, "Avaria atualizada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleDamageResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar avaria");
            return StatusCode(500, ApiResponse<VehicleDamageResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Deletar avaria
    /// </summary>
    [HttpDelete("{vehicleId}/management/damages/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDamage(Guid vehicleId, Guid id)
    {
        try
        {
            await _vehicleManagementService.DeleteDamageAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Avaria deletada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar avaria");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Reparar avaria
    /// </summary>
    [HttpPost("{vehicleId}/management/damages/{id}/repair")]
    public async Task<ActionResult<ApiResponse<VehicleDamageResponse>>> RepairDamage(Guid vehicleId, Guid id, [FromBody] RepairVehicleDamageRequest request)
    {
        try
        {
            var response = await _vehicleManagementService.RepairDamageAsync(id, request);
            return Ok(ApiResponse<VehicleDamageResponse>.SuccessResponse(response, "Avaria reparada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleDamageResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao reparar avaria");
            return StatusCode(500, ApiResponse<VehicleDamageResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    // ============ MILEAGE LOGS ============

    /// <summary>
    /// Listar registros de quilometragem do veículo
    /// </summary>
    [HttpGet("{id}/management/mileage-logs")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleMileageLogResponse>>>> GetMileageLogs(Guid id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var response = await _vehicleManagementService.GetMileageLogsByVehicleAsync(id, startDate, endDate);
            return Ok(ApiResponse<IEnumerable<VehicleMileageLogResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar registros de quilometragem");
            return StatusCode(500, ApiResponse<IEnumerable<VehicleMileageLogResponse>>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Obter registro de quilometragem por ID
    /// </summary>
    [HttpGet("{vehicleId}/management/mileage-logs/{id}")]
    public async Task<ActionResult<ApiResponse<VehicleMileageLogResponse>>> GetMileageLog(Guid vehicleId, Guid id)
    {
        try
        {
            var response = await _vehicleManagementService.GetMileageLogByIdAsync(id);
            return Ok(ApiResponse<VehicleMileageLogResponse>.SuccessResponse(response));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleMileageLogResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter registro de quilometragem");
            return StatusCode(500, ApiResponse<VehicleMileageLogResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Criar registro de quilometragem
    /// </summary>
    [HttpPost("{id}/management/mileage-logs")]
    public async Task<ActionResult<ApiResponse<VehicleMileageLogResponse>>> CreateMileageLog(Guid id, [FromBody] CreateVehicleMileageLogRequest request)
    {
        try
        {
            request.VehicleId = id;
            var response = await _vehicleManagementService.CreateMileageLogAsync(request);
            return CreatedAtAction(nameof(GetMileageLog), new { vehicleId = id, id = response.Id }, 
                ApiResponse<VehicleMileageLogResponse>.SuccessResponse(response, "Registro de quilometragem criado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleMileageLogResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar registro de quilometragem");
            return StatusCode(500, ApiResponse<VehicleMileageLogResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Completar registro de quilometragem (finalizar viagem)
    /// </summary>
    [HttpPost("{vehicleId}/management/mileage-logs/{id}/complete")]
    public async Task<ActionResult<ApiResponse<VehicleMileageLogResponse>>> CompleteMileageLog(Guid vehicleId, Guid id, [FromBody] CompleteMileageLogRequest request)
    {
        try
        {
            var response = await _vehicleManagementService.CompleteMileageLogAsync(id, request);
            return Ok(ApiResponse<VehicleMileageLogResponse>.SuccessResponse(response, "Viagem finalizada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleMileageLogResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao finalizar viagem");
            return StatusCode(500, ApiResponse<VehicleMileageLogResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Cancelar registro de quilometragem
    /// </summary>
    [HttpPost("{vehicleId}/management/mileage-logs/{id}/cancel")]
    public async Task<ActionResult<ApiResponse<VehicleMileageLogResponse>>> CancelMileageLog(Guid vehicleId, Guid id, [FromBody] CancelMileageLogRequest? request)
    {
        try
        {
            var response = await _vehicleManagementService.CancelMileageLogAsync(id, request?.Reason);
            return Ok(ApiResponse<VehicleMileageLogResponse>.SuccessResponse(response, "Viagem cancelada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<VehicleMileageLogResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar viagem");
            return StatusCode(500, ApiResponse<VehicleMileageLogResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Deletar registro de quilometragem
    /// </summary>
    [HttpDelete("{vehicleId}/management/mileage-logs/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMileageLog(Guid vehicleId, Guid id)
    {
        try
        {
            await _vehicleManagementService.DeleteMileageLogAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Registro deletado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar registro de quilometragem");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Erro interno no servidor"));
        }
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class CancelMileageLogRequest
{
    public string? Reason { get; set; }
}
