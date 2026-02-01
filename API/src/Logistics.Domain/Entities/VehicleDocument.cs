namespace Logistics.Domain.Entities;

/// <summary>
/// Representa um documento do veículo (seguro, licenciamento, etc)
/// </summary>
public class VehicleDocument
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid CompanyId { get; private set; }
    
    // Tipo de documento
    public VehicleDocumentType Type { get; private set; }
    public string DocumentNumber { get; private set; }
    public string? Description { get; private set; }
    
    // Datas
    public DateTime IssueDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    
    // Emissor
    public string? IssuingAuthority { get; private set; }
    
    // Arquivo anexo
    public string? FileName { get; private set; }
    public string? FilePath { get; private set; }
    public string? FileType { get; private set; }
    
    // Custo
    public decimal? Cost { get; private set; }
    
    // Alertas
    public bool AlertOnExpiry { get; private set; }
    public int? AlertDaysBefore { get; private set; }
    
    public string? Notes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation
    public Vehicle Vehicle { get; private set; } = null!;
    public Company Company { get; private set; } = null!;

    private VehicleDocument() { }

    public VehicleDocument(
        Guid vehicleId,
        Guid companyId,
        VehicleDocumentType type,
        string documentNumber,
        DateTime issueDate,
        DateTime? expiryDate = null,
        string? description = null,
        string? issuingAuthority = null,
        decimal? cost = null,
        bool alertOnExpiry = true,
        int? alertDaysBefore = 30)
    {
        Id = Guid.NewGuid();
        VehicleId = vehicleId;
        CompanyId = companyId;
        Type = type;
        DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        Description = description;
        IssuingAuthority = issuingAuthority;
        Cost = cost;
        AlertOnExpiry = alertOnExpiry;
        AlertDaysBefore = alertDaysBefore;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        VehicleDocumentType type,
        string documentNumber,
        DateTime issueDate,
        DateTime? expiryDate,
        string? description,
        string? issuingAuthority,
        decimal? cost,
        bool alertOnExpiry,
        int? alertDaysBefore,
        string? notes)
    {
        Type = type;
        DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        Description = description;
        IssuingAuthority = issuingAuthority;
        Cost = cost;
        AlertOnExpiry = alertOnExpiry;
        AlertDaysBefore = alertDaysBefore;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachFile(string fileName, string filePath, string fileType)
    {
        FileName = fileName;
        FilePath = filePath;
        FileType = fileType;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFile(string fileName, string filePath, string fileType)
    {
        AttachFile(fileName, filePath, fileType);
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    public bool IsExpiringSoon => ExpiryDate.HasValue && 
                                   ExpiryDate.Value < DateTime.UtcNow.AddDays(AlertDaysBefore ?? 30) &&
                                   !IsExpired;
}

/// <summary>
/// Tipos de documentos do veículo
/// </summary>
public enum VehicleDocumentType
{
    // Documentos de propriedade
    RegistrationCertificate = 0,  // CRV/CRLV (BR), Permiso de Circulación (ES), V5C (UK)
    OwnershipTitle = 1,           // Título de propriedade
    
    // Seguros
    Insurance = 10,               // Seguro do veículo
    ThirdPartyInsurance = 11,     // Seguro contra terceiros
    CargoInsurance = 12,          // Seguro de carga
    
    // Licenças e habilitações
    OperatingLicense = 20,        // Licença de operação
    TransportLicense = 21,        // Licença de transporte
    HazmatLicense = 22,           // Licença para produtos perigosos
    
    // Fiscais
    RoadTax = 30,                 // IPVA (BR), Impuesto de Circulación (ES)
    TollTag = 31,                 // Tag de pedágio
    
    // Técnicos
    TachographCertificate = 40,   // Certificado do tacógrafo
    SpeedometerCertificate = 41,  // Certificado do velocímetro
    
    // Outros
    PurchaseInvoice = 50,         // Nota fiscal de compra
    Warranty = 51,                // Garantia
    Manual = 52,                  // Manual do veículo
    Other = 99
}
