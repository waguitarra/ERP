namespace Logistics.Domain.Entities;

/// <summary>
/// Representa uma inspeção veicular (ITV/MOT/Inspeção Anual)
/// </summary>
public class VehicleInspection
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid CompanyId { get; private set; }
    
    // Tipo de inspeção por país
    public InspectionType Type { get; private set; }
    
    // Datas
    public DateTime InspectionDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    
    // Resultado
    public InspectionResult Result { get; private set; }
    
    // Centro de inspeção
    public string? InspectionCenter { get; private set; }
    public string? InspectorName { get; private set; }
    public string? CertificateNumber { get; private set; }
    
    // Quilometragem
    public decimal MileageAtInspection { get; private set; }
    
    // Custos
    public decimal Cost { get; private set; }
    
    // Observações/Defeitos encontrados
    public string? Observations { get; private set; }
    public string? DefectsFound { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation
    public Vehicle Vehicle { get; private set; }
    public Company Company { get; private set; }

    private VehicleInspection() { }

    public VehicleInspection(
        Guid vehicleId,
        Guid companyId,
        InspectionType type,
        DateTime inspectionDate,
        DateTime expiryDate,
        InspectionResult result,
        decimal mileageAtInspection,
        decimal cost = 0,
        string? inspectionCenter = null,
        string? certificateNumber = null,
        string? observations = null)
    {
        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        CompanyId = companyId;
        Type = type;
        InspectionDate = inspectionDate;
        ExpiryDate = expiryDate;
        Result = result;
        MileageAtInspection = mileageAtInspection;
        Cost = cost;
        InspectionCenter = inspectionCenter;
        CertificateNumber = certificateNumber;
        Observations = observations;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        InspectionType type,
        DateTime inspectionDate,
        DateTime expiryDate,
        InspectionResult result,
        decimal mileageAtInspection,
        decimal cost,
        string? inspectionCenter,
        string? inspectorName,
        string? certificateNumber,
        string? observations,
        string? defectsFound)
    {
        Type = type;
        InspectionDate = inspectionDate;
        ExpiryDate = expiryDate;
        Result = result;
        MileageAtInspection = mileageAtInspection;
        Cost = cost;
        InspectionCenter = inspectionCenter;
        InspectorName = inspectorName;
        CertificateNumber = certificateNumber;
        Observations = observations;
        DefectsFound = defectsFound;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDefects(string defectsFound)
    {
        DefectsFound = defectsFound;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired => ExpiryDate < DateTime.UtcNow;
    public bool IsExpiringSoon => ExpiryDate < DateTime.UtcNow.AddDays(30) && !IsExpired;
}

/// <summary>
/// Tipos de inspeção por país
/// </summary>
public enum InspectionType
{
    ITV = 0,           // Espanha - Inspección Técnica de Vehículos
    MOT = 1,           // UK - Ministry of Transport test
    TUV = 2,           // Alemanha - Technischer Überwachungsverein
    CT = 3,            // França - Contrôle Technique
    Revisione = 4,     // Itália - Revisione
    APK = 5,           // Holanda - Algemene Periodieke Keuring
    DETRAN = 6,        // Brasil - Inspeção Veicular DETRAN
    SafetyInspection = 7, // USA - Safety Inspection
    EmissionsTest = 8,    // USA - Emissions Test
    Other = 99
}

/// <summary>
/// Resultado da inspeção
/// </summary>
public enum InspectionResult
{
    Approved = 0,           // Aprovado
    ApprovedWithDefects = 1, // Aprovado com defeitos leves
    Rejected = 2,           // Reprovado
    Pending = 3             // Pendente de re-inspeção
}
