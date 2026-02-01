namespace Logistics.Domain.Entities;

/// <summary>
/// Representa um registro de manutenção do veículo
/// </summary>
public class VehicleMaintenance
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid CompanyId { get; private set; }
    
    // Tipo de manutenção
    public MaintenanceType Type { get; private set; }
    public string Description { get; private set; }
    
    // Datas
    public DateTime MaintenanceDate { get; private set; }
    public DateTime? NextMaintenanceDate { get; private set; }
    
    // Quilometragem
    public decimal MileageAtMaintenance { get; private set; }
    public decimal? NextMaintenanceMileage { get; private set; }
    
    // Custos
    public decimal LaborCost { get; private set; }
    public decimal PartsCost { get; private set; }
    public decimal TotalCost => LaborCost + PartsCost;
    
    // Fornecedor/Oficina
    public string? ServiceProvider { get; private set; }
    public string? ServiceProviderContact { get; private set; }
    
    // Documentação
    public string? InvoiceNumber { get; private set; }
    public string? Notes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation
    public Vehicle Vehicle { get; private set; }
    public Company Company { get; private set; }

    private VehicleMaintenance() { }

    public VehicleMaintenance(
        Guid vehicleId, 
        Guid companyId,
        MaintenanceType type,
        string description,
        DateTime maintenanceDate,
        decimal mileageAtMaintenance,
        decimal laborCost = 0,
        decimal partsCost = 0,
        string? serviceProvider = null,
        string? invoiceNumber = null,
        string? notes = null)
    {
        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        CompanyId = companyId;
        Type = type;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        MaintenanceDate = maintenanceDate;
        MileageAtMaintenance = mileageAtMaintenance;
        LaborCost = laborCost;
        PartsCost = partsCost;
        ServiceProvider = serviceProvider;
        InvoiceNumber = invoiceNumber;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        MaintenanceType type,
        string description,
        DateTime maintenanceDate,
        decimal mileageAtMaintenance,
        decimal laborCost,
        decimal partsCost,
        string? serviceProvider,
        string? serviceProviderContact,
        string? invoiceNumber,
        string? notes,
        DateTime? nextMaintenanceDate = null,
        decimal? nextMaintenanceMileage = null)
    {
        Type = type;
        Description = description;
        MaintenanceDate = maintenanceDate;
        MileageAtMaintenance = mileageAtMaintenance;
        LaborCost = laborCost;
        PartsCost = partsCost;
        ServiceProvider = serviceProvider;
        ServiceProviderContact = serviceProviderContact;
        InvoiceNumber = invoiceNumber;
        Notes = notes;
        NextMaintenanceDate = nextMaintenanceDate;
        NextMaintenanceMileage = nextMaintenanceMileage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetNextMaintenance(DateTime nextDate, decimal? nextMileage = null)
    {
        NextMaintenanceDate = nextDate;
        NextMaintenanceMileage = nextMileage;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Tipos de manutenção
/// </summary>
public enum MaintenanceType
{
    Preventive = 0,      // Preventiva (revisão periódica)
    Corrective = 1,      // Corretiva (reparo)
    OilChange = 2,       // Troca de óleo
    TireChange = 3,      // Troca de pneus
    BrakeService = 4,    // Serviço de freios
    EngineRepair = 5,    // Reparo no motor
    Transmission = 6,    // Transmissão
    Electrical = 7,      // Elétrica
    BodyWork = 8,        // Funilaria/Pintura
    AirConditioning = 9, // Ar condicionado
    Other = 99           // Outros
}
