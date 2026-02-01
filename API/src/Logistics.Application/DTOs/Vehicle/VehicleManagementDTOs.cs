using Logistics.Domain.Entities;

namespace Logistics.Application.DTOs.Vehicle;

// ===================== MAINTENANCE =====================

public class VehicleMaintenanceResponse
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public MaintenanceType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime MaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public decimal MileageAtMaintenance { get; set; }
    public decimal? NextMaintenanceMileage { get; set; }
    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? ServiceProvider { get; set; }
    public string? ServiceProviderContact { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleMaintenanceRequest
{
    public Guid VehicleId { get; set; }
    public MaintenanceType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime MaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public decimal MileageAtMaintenance { get; set; }
    public decimal? NextMaintenanceMileage { get; set; }
    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }
    public string? ServiceProvider { get; set; }
    public string? ServiceProviderContact { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Notes { get; set; }
}

public class UpdateVehicleMaintenanceRequest : CreateVehicleMaintenanceRequest
{
    public Guid Id { get; set; }
}

// ===================== INSPECTION =====================

public class VehicleInspectionResponse
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public InspectionType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public InspectionResult Result { get; set; }
    public string ResultName { get; set; } = string.Empty;
    public string? InspectionCenter { get; set; }
    public string? InspectorName { get; set; }
    public string? CertificateNumber { get; set; }
    public decimal MileageAtInspection { get; set; }
    public decimal Cost { get; set; }
    public string? Observations { get; set; }
    public string? DefectsFound { get; set; }
    public bool IsExpired { get; set; }
    public bool IsExpiringSoon { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleInspectionRequest
{
    public Guid VehicleId { get; set; }
    public InspectionType Type { get; set; }
    public DateTime InspectionDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public InspectionResult Result { get; set; }
    public string? InspectionCenter { get; set; }
    public string? InspectorName { get; set; }
    public string? CertificateNumber { get; set; }
    public decimal MileageAtInspection { get; set; }
    public decimal Cost { get; set; }
    public string? Observations { get; set; }
    public string? DefectsFound { get; set; }
}

public class UpdateVehicleInspectionRequest : CreateVehicleInspectionRequest
{
    public Guid Id { get; set; }
}

// ===================== DOCUMENT =====================

public class VehicleDocumentResponse
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public VehicleDocumentType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
    public decimal? Cost { get; set; }
    public bool AlertOnExpiry { get; set; }
    public int? AlertDaysBefore { get; set; }
    public string? Notes { get; set; }
    public bool IsExpired { get; set; }
    public bool IsExpiringSoon { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleDocumentRequest
{
    public Guid VehicleId { get; set; }
    public VehicleDocumentType Type { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public decimal? Cost { get; set; }
    public bool AlertOnExpiry { get; set; } = true;
    public int? AlertDaysBefore { get; set; } = 30;
    public string? Notes { get; set; }
}

public class UpdateVehicleDocumentRequest : CreateVehicleDocumentRequest
{
    public Guid Id { get; set; }
}

// ===================== VEHICLE DETAIL =====================

public class VehicleDetailResponse : VehicleResponse
{
    public List<VehicleMaintenanceResponse> RecentMaintenances { get; set; } = new();
    public List<VehicleInspectionResponse> Inspections { get; set; } = new();
    public List<VehicleDocumentResponse> Documents { get; set; } = new();
    public List<VehicleDeliveryHistoryResponse> DeliveryHistory { get; set; } = new();
}

public class VehicleDeliveryHistoryResponse
{
    public Guid ShipmentId { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public DateTime ShipmentDate { get; set; }
    public string? CustomerName { get; set; }
    public string? DeliveryAddress { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? Distance { get; set; }
}

// ===================== VEHICLE UPDATE REQUESTS =====================

public class UpdateVehicleFinancialRequest
{
    public decimal? PurchasePrice { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? CurrentValue { get; set; }
    public string? ChassisNumber { get; set; }
    public string? EngineNumber { get; set; }
}

public class UpdateVehicleMileageRequest
{
    public decimal CurrentMileage { get; set; }
}

public class UpdateVehicleInsuranceRequest
{
    public DateTime? InsuranceExpiryDate { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
}
// ===================== DAMAGE =====================

public class VehicleDamageResponse
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DamageType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public DamageSeverity Severity { get; set; }
    public string SeverityName { get; set; } = string.Empty;
    public string? DamageLocation { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public DateTime? ReportedDate { get; set; }
    public DateTime? RepairedDate { get; set; }
    public decimal MileageAtOccurrence { get; set; }
    public DamageStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public decimal EstimatedRepairCost { get; set; }
    public decimal ActualRepairCost { get; set; }
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public bool IsThirdPartyFault { get; set; }
    public string? ThirdPartyInfo { get; set; }
    public bool InsuranceClaim { get; set; }
    public string? InsuranceClaimNumber { get; set; }
    public decimal? InsuranceReimbursement { get; set; }
    public string? RepairShop { get; set; }
    public string? RepairNotes { get; set; }
    public List<string> PhotoUrls { get; set; } = new();
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleDamageRequest
{
    public Guid VehicleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DamageType Type { get; set; }
    public DamageSeverity Severity { get; set; }
    public string? DamageLocation { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public decimal MileageAtOccurrence { get; set; }
    public decimal EstimatedRepairCost { get; set; }
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public bool IsThirdPartyFault { get; set; }
    public string? ThirdPartyInfo { get; set; }
    public string? Notes { get; set; }
}

public class UpdateVehicleDamageRequest : CreateVehicleDamageRequest
{
    public Guid Id { get; set; }
}

public class RepairVehicleDamageRequest
{
    public string RepairShop { get; set; } = string.Empty;
    public decimal ActualRepairCost { get; set; }
    public DateTime RepairedDate { get; set; }
    public string? RepairNotes { get; set; }
}

public class VehicleDamageInsuranceRequest
{
    public string ClaimNumber { get; set; } = string.Empty;
    public decimal? Reimbursement { get; set; }
}

public class VehicleDamagePhotosRequest
{
    public List<string> PhotoUrls { get; set; } = new();
}

// ===================== MILEAGE LOG =====================

public class VehicleMileageLogResponse
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public MileageLogType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public decimal StartMileage { get; set; }
    public decimal EndMileage { get; set; }
    public decimal Distance { get; set; }
    public double? StartLatitude { get; set; }
    public double? StartLongitude { get; set; }
    public string? StartAddress { get; set; }
    public double? EndLatitude { get; set; }
    public double? EndLongitude { get; set; }
    public string? EndAddress { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public Guid? ShipmentId { get; set; }
    public string? ShipmentNumber { get; set; }
    public decimal? FuelConsumed { get; set; }
    public decimal? FuelCost { get; set; }
    public decimal? FuelEfficiency { get; set; }
    public MileageLogStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public decimal? TollCost { get; set; }
    public decimal? ParkingCost { get; set; }
    public decimal? OtherCosts { get; set; }
    public decimal TotalCost { get; set; }
    public string? Purpose { get; set; }
    public string? Notes { get; set; }
    public string? RoutePolyline { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVehicleMileageLogRequest
{
    public Guid VehicleId { get; set; }
    public MileageLogType Type { get; set; }
    public decimal StartMileage { get; set; }
    public DateTime StartDateTime { get; set; }
    public double? StartLatitude { get; set; }
    public double? StartLongitude { get; set; }
    public string? StartAddress { get; set; }
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public Guid? ShipmentId { get; set; }
    public string? ShipmentNumber { get; set; }
    public string? Purpose { get; set; }
}

public class CompleteMileageLogRequest
{
    public decimal EndMileage { get; set; }
    public DateTime EndDateTime { get; set; }
    public double? EndLatitude { get; set; }
    public double? EndLongitude { get; set; }
    public string? EndAddress { get; set; }
    public string? RoutePolyline { get; set; }
    public decimal? FuelConsumed { get; set; }
    public decimal? FuelCost { get; set; }
    public decimal? TollCost { get; set; }
    public decimal? ParkingCost { get; set; }
    public decimal? OtherCosts { get; set; }
}

public class UpdateMileageLogCostsRequest
{
    public decimal? FuelConsumed { get; set; }
    public decimal? FuelCost { get; set; }
    public decimal? TollCost { get; set; }
    public decimal? ParkingCost { get; set; }
    public decimal? OtherCosts { get; set; }
}

// ===================== ALERTS & SUMMARY =====================

public class VehicleAlertResponse
{
    public Guid VehicleId { get; set; }
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int? DaysUntilDue { get; set; }
    public string Severity { get; set; } = string.Empty;
}

public class VehicleSummaryResponse
{
    public Guid VehicleId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal CurrentMileage { get; set; }
    public decimal TotalMaintenanceCost { get; set; }
    public decimal TotalDamageCost { get; set; }
    public int TotalDeliveries { get; set; }
    public int PendingDamages { get; set; }
    public int ExpiringDocuments { get; set; }
    public int ExpiringInspections { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public DateTime? NextInspectionDate { get; set; }
    public List<VehicleAlertResponse> Alerts { get; set; } = new();
}