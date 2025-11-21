namespace Logistics.Application.DTOs.User;

public class UserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public int Role { get; set; } // 0=Admin, 1=CompanyAdmin, 2=CompanyUser
}
