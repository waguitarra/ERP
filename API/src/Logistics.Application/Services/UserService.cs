using Logistics.Application.DTOs.User;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
            throw new KeyNotFoundException("Empresa não encontrada");

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email já cadastrado");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Name, request.Email, passwordHash, request.Role, request.CompanyId);

        await _userRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        return MapToResponse(user);
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado");

        return MapToResponse(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToResponse);
    }

    public async Task<IEnumerable<UserResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var users = await _userRepository.GetByCompanyIdAsync(companyId);
        return users.Select(MapToResponse);
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado");

        user.Update(request.Name, request.Email);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();

        return MapToResponse(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado");

        await _userRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<UserResponse> UpdateRoleAsync(Guid id, int role)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("Usuário não encontrado");

        user.UpdateRole((UserRole)role);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();

        return MapToResponse(user);
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            CompanyId = user.CompanyId,
            CompanyName = user.Company?.Name ?? "",
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
