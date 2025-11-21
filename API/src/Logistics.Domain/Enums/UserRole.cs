namespace Logistics.Domain.Enums;

public enum UserRole
{
    Admin = 0,        // Master admin - acesso total
    CompanyAdmin = 1, // Admin da empresa
    CompanyUser = 2   // Usuário operacional
}
