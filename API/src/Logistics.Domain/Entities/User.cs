using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public Guid? CompanyId { get; private set; } // Null para Admin Master
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Navigation property
    public Company? Company { get; private set; }

    // Constructor privado para EF
    private User() { }

    public User(string name, string email, string passwordHash, UserRole role, Guid? companyId = null)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        Role = role;
        CompanyId = companyId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Update(string name, string email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        UpdatedAt = DateTime.UtcNow;
        
        Validate();
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Nome é obrigatório", nameof(Name));

        if (string.IsNullOrWhiteSpace(Email))
            throw new ArgumentException("Email é obrigatório", nameof(Email));

        if (!Email.Contains("@"))
            throw new ArgumentException("Email inválido", nameof(Email));

        // Validação: Admin Master não deve ter CompanyId
        if (Role == UserRole.Admin && CompanyId.HasValue)
            throw new InvalidOperationException("Admin Master não pode estar vinculado a uma empresa");

        // Validação: Usuários de empresa devem ter CompanyId
        if (Role != UserRole.Admin && !CompanyId.HasValue)
            throw new InvalidOperationException("Usuários de empresa devem estar vinculados a uma empresa");
    }
}
