namespace Logistics.Domain.Entities;

public class Driver
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string LicenseNumber { get; private set; }
    public string Phone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation property
    public Company Company { get; private set; }

    // Constructor privado para EF
    private Driver() { }

    public Driver(Guid companyId, string name, string licenseNumber, string phone)
    {
        Id = Guid.NewGuid();
        CompanyId = companyId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        LicenseNumber = licenseNumber ?? throw new ArgumentNullException(nameof(licenseNumber));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Update(string name, string licenseNumber, string phone)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        LicenseNumber = licenseNumber ?? throw new ArgumentNullException(nameof(licenseNumber));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        UpdatedAt = DateTime.UtcNow;
        
        Validate();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Nome é obrigatório", nameof(Name));

        if (string.IsNullOrWhiteSpace(LicenseNumber))
            throw new ArgumentException("Número da CNH é obrigatório", nameof(LicenseNumber));

        if (string.IsNullOrWhiteSpace(Phone))
            throw new ArgumentException("Telefone é obrigatório", nameof(Phone));

        if (CompanyId == Guid.Empty)
            throw new ArgumentException("CompanyId é obrigatório", nameof(CompanyId));
    }
}
