namespace Logistics.Domain.Entities;

public class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Document { get; private set; } // CNPJ
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public ICollection<User> Users { get; private set; }
    public ICollection<Vehicle> Vehicles { get; private set; }
    public ICollection<Driver> Drivers { get; private set; }

    // Constructor privado para EF
    private Company() 
    { 
        Users = new List<User>();
        Vehicles = new List<Vehicle>();
        Drivers = new List<Driver>();
    }

    public Company(string name, string document)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Document = document ?? throw new ArgumentNullException(nameof(document));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        
        Users = new List<User>();
        Vehicles = new List<Vehicle>();
        Drivers = new List<Driver>();

        Validate();
    }

    public void Update(string name, string document)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Document = document ?? throw new ArgumentNullException(nameof(document));
        UpdatedAt = DateTime.UtcNow;
        
        Validate();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Nome da empresa é obrigatório", nameof(Name));

        if (string.IsNullOrWhiteSpace(Document))
            throw new ArgumentException("Documento da empresa é obrigatório", nameof(Document));

        // Validação básica de CNPJ (14 dígitos)
        var cleanDocument = Document.Replace(".", "").Replace("/", "").Replace("-", "");
        if (cleanDocument.Length != 14)
            throw new ArgumentException("Documento deve ser um CNPJ válido (14 dígitos)", nameof(Document));
    }
}
