using Logistics.Application.DTOs.Auth;
using Logistics.Application.DTOs.Common;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login de usuário
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Login realizado com sucesso"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Tentativa de login falhou para email: {Email}", request.Email);
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login");
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Registrar administrador master (apenas primeiro acesso)
    /// </summary>
    [HttpPost("register-admin")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RegisterAdmin([FromBody] RegisterAdminRequest request)
    {
        try
        {
            var response = await _authService.RegisterAdminAsync(request);
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Administrador criado com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Tentativa de registrar admin duplicado");
            return BadRequest(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar administrador");
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }
}
