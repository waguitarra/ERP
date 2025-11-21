namespace Logistics.Domain.Entities;

public class Customer
{
    private Customer() { }

    public Customer(Guid companyId, string name, string document, string? phone = null, string? email = null)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException("Documento não pode ser vazio");

        Id = Guid.NewGuid();
        CompanyId = companyId;
        Name = name;
        Document = document;
        Phone = phone;
        Email = email;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Company Company { get; private set; } = null!;

    public void Update(string name, string document, string? phone, string? email, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(document))
            throw new ArgumentException("Documento não pode ser vazio");

        Name = name;
        Document = document;
        Phone = phone;
        Email = email;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
