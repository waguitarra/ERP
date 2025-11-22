using Logistics.Application.DTOs.Common;
using Logistics.Application.DTOs.User;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Criar novo usuário
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateAsync(request);
            return Ok(ApiResponse<UserResponse>.SuccessResponse(user, "Usuário criado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<UserResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário");
            return StatusCode(500, ApiResponse<UserResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }

    /// <summary>
    /// Buscar usuário por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(Guid id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(ApiResponse<UserResponse>.SuccessResponse(user));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<UserResponse>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Listar todos os usuários
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<UserResponse>>.SuccessResponse(users));
    }

    /// <summary>
    /// Listar usuários por empresa
    /// </summary>
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetByCompanyId(Guid companyId)
    {
        var users = await _userService.GetByCompanyIdAsync(companyId);
        return Ok(ApiResponse<IEnumerable<UserResponse>>.SuccessResponse(users));
    }

    /// <summary>
    /// Atualizar usuário
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, request);
            return Ok(ApiResponse<UserResponse>.SuccessResponse(user, "Usuário atualizado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<UserResponse>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Atualizar role do usuário
    /// </summary>
    [HttpPatch("{id}/role")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateRole(Guid id, [FromBody] int role)
    {
        try
        {
            var user = await _userService.UpdateRoleAsync(id, role);
            return Ok(ApiResponse<UserResponse>.SuccessResponse(user, "Role atualizado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<UserResponse>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Deletar usuário
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Usuário deletado com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }
}
