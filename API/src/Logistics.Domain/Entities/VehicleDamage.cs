namespace Logistics.Domain.Entities;

/// <summary>
/// Representa um registro de avaria/dano do veículo com fotos e custos
/// </summary>
public class VehicleDamage
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid CompanyId { get; private set; }
    
    // Descrição da avaria
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DamageType Type { get; private set; }
    public DamageSeverity Severity { get; private set; }
    
    // Localização no veículo
    public string? DamageLocation { get; private set; }  // Ex: "Porta dianteira esquerda", "Para-choque traseiro"
    
    // Datas
    public DateTime OccurrenceDate { get; private set; }
    public DateTime? ReportedDate { get; private set; }
    public DateTime? RepairedDate { get; private set; }
    
    // Quilometragem no momento da avaria
    public decimal MileageAtOccurrence { get; private set; }
    
    // Status
    public DamageStatus Status { get; private set; }
    
    // Custos
    public decimal EstimatedRepairCost { get; private set; }
    public decimal ActualRepairCost { get; private set; }
    
    // Responsável/Causador
    public Guid? DriverId { get; private set; }
    public string? DriverName { get; private set; }
    public bool IsThirdPartyFault { get; private set; }
    public string? ThirdPartyInfo { get; private set; }
    
    // Seguro
    public bool InsuranceClaim { get; private set; }
    public string? InsuranceClaimNumber { get; private set; }
    public decimal? InsuranceReimbursement { get; private set; }
    
    // Reparação
    public string? RepairShop { get; private set; }
    public string? RepairNotes { get; private set; }
    
    // Fotos - armazenadas como JSON array de URLs
    public string? PhotoUrls { get; private set; }  // JSON: ["url1.webp", "url2.webp"]
    
    public string? Notes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation
    public Vehicle Vehicle { get; private set; }
    public Company Company { get; private set; }
    public Driver? Driver { get; private set; }

    private VehicleDamage() { }

    public VehicleDamage(
        Guid vehicleId,
        Guid companyId,
        string title,
        string description,
        DamageType type,
        DamageSeverity severity,
        DateTime occurrenceDate,
        decimal mileageAtOccurrence,
        string? damageLocation = null,
        Guid? driverId = null,
        string? driverName = null)
    {
        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        CompanyId = companyId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        Severity = severity;
        OccurrenceDate = occurrenceDate;
        MileageAtOccurrence = mileageAtOccurrence;
        DamageLocation = damageLocation;
        DriverId = driverId;
        DriverName = driverName;
        Status = DamageStatus.Reported;
        ReportedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        string title,
        string description,
        DamageType type,
        DamageSeverity severity,
        DateTime occurrenceDate,
        decimal mileageAtOccurrence,
        string? damageLocation,
        decimal estimatedRepairCost,
        bool isThirdPartyFault,
        string? thirdPartyInfo)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        Severity = severity;
        OccurrenceDate = occurrenceDate;
        MileageAtOccurrence = mileageAtOccurrence;
        DamageLocation = damageLocation;
        EstimatedRepairCost = estimatedRepairCost;
        IsThirdPartyFault = isThirdPartyFault;
        ThirdPartyInfo = thirdPartyInfo;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRepairInfo(
        string repairShop,
        decimal actualRepairCost,
        DateTime repairedDate,
        string? repairNotes = null)
    {
        RepairShop = repairShop;
        ActualRepairCost = actualRepairCost;
        RepairedDate = repairedDate;
        RepairNotes = repairNotes;
        Status = DamageStatus.Repaired;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetInsuranceClaim(
        string claimNumber,
        decimal? reimbursement = null)
    {
        InsuranceClaim = true;
        InsuranceClaimNumber = claimNumber;
        InsuranceReimbursement = reimbursement;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePhotos(string photoUrlsJson)
    {
        PhotoUrls = photoUrlsJson;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(DamageStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnderRepair()
    {
        Status = DamageStatus.UnderRepair;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsWriteOff()
    {
        Status = DamageStatus.WriteOff;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum DamageType
{
    Collision = 0,          // Colisão
    Scratch = 1,            // Arranhão
    Dent = 2,               // Amassado
    BrokenGlass = 3,        // Vidro quebrado
    MechanicalFailure = 4,  // Falha mecânica
    Vandalism = 5,          // Vandalismo
    WeatherDamage = 6,      // Dano por tempo
    WearAndTear = 7,        // Desgaste
    Theft = 8,              // Roubo/Furto parcial
    Other = 99
}

public enum DamageSeverity
{
    Minor = 0,      // Menor - cosmético
    Moderate = 1,   // Moderado - afeta uso
    Major = 2,      // Maior - impede uso
    Critical = 3    // Crítico - veículo inoperante
}

public enum DamageStatus
{
    Reported = 0,       // Reportado
    UnderAssessment = 1, // Em avaliação
    UnderRepair = 2,    // Em reparo
    Repaired = 3,       // Reparado
    WriteOff = 4        // Perda total
}
