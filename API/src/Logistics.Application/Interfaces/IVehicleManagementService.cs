using Logistics.Application.DTOs.Vehicle;
using Logistics.Domain.Entities;

namespace Logistics.Application.Interfaces;

public interface IVehicleManagementService
{
    // Maintenance
    Task<VehicleMaintenanceResponse> CreateMaintenanceAsync(CreateVehicleMaintenanceRequest request);
    Task<VehicleMaintenanceResponse> UpdateMaintenanceAsync(Guid id, UpdateVehicleMaintenanceRequest request);
    Task DeleteMaintenanceAsync(Guid id);
    Task<VehicleMaintenanceResponse> GetMaintenanceByIdAsync(Guid id);
    Task<IEnumerable<VehicleMaintenanceResponse>> GetMaintenancesByVehicleAsync(Guid vehicleId);
    
    // Inspection
    Task<VehicleInspectionResponse> CreateInspectionAsync(CreateVehicleInspectionRequest request);
    Task<VehicleInspectionResponse> UpdateInspectionAsync(Guid id, UpdateVehicleInspectionRequest request);
    Task DeleteInspectionAsync(Guid id);
    Task<VehicleInspectionResponse> GetInspectionByIdAsync(Guid id);
    Task<IEnumerable<VehicleInspectionResponse>> GetInspectionsByVehicleAsync(Guid vehicleId);
    Task<IEnumerable<VehicleInspectionResponse>> GetExpiringInspectionsAsync(int daysAhead = 30);
    
    // Document
    Task<VehicleDocumentResponse> CreateDocumentAsync(CreateVehicleDocumentRequest request);
    Task<VehicleDocumentResponse> UpdateDocumentAsync(Guid id, UpdateVehicleDocumentRequest request);
    Task DeleteDocumentAsync(Guid id);
    Task<VehicleDocumentResponse> GetDocumentByIdAsync(Guid id);
    Task<IEnumerable<VehicleDocumentResponse>> GetDocumentsByVehicleAsync(Guid vehicleId);
    Task<IEnumerable<VehicleDocumentResponse>> GetExpiringDocumentsAsync(int daysAhead = 30);
    Task<VehicleDocumentResponse> UploadDocumentFileAsync(Guid id, string fileName, string filePath, string fileType);
    
    // Damage
    Task<VehicleDamageResponse> CreateDamageAsync(CreateVehicleDamageRequest request);
    Task<VehicleDamageResponse> UpdateDamageAsync(Guid id, UpdateVehicleDamageRequest request);
    Task DeleteDamageAsync(Guid id);
    Task<VehicleDamageResponse> GetDamageByIdAsync(Guid id);
    Task<IEnumerable<VehicleDamageResponse>> GetAllDamagesAsync(Guid? companyId = null);
    Task<IEnumerable<VehicleDamageResponse>> GetDamagesByVehicleAsync(Guid vehicleId);
    Task<IEnumerable<VehicleDamageResponse>> GetPendingDamagesAsync();
    Task<VehicleDamageResponse> RepairDamageAsync(Guid id, RepairVehicleDamageRequest request);
    Task<VehicleDamageResponse> SetInsuranceClaimAsync(Guid id, VehicleDamageInsuranceRequest request);
    Task<VehicleDamageResponse> UpdateDamagePhotosAsync(Guid id, VehicleDamagePhotosRequest request);
    Task<VehicleDamageResponse> UpdateDamageStatusAsync(Guid id, DamageStatus status);
    
    // Mileage Log
    Task<VehicleMileageLogResponse> CreateMileageLogAsync(CreateVehicleMileageLogRequest request);
    Task<VehicleMileageLogResponse> CompleteMileageLogAsync(Guid id, CompleteMileageLogRequest request);
    Task<VehicleMileageLogResponse> UpdateMileageLogCostsAsync(Guid id, UpdateMileageLogCostsRequest request);
    Task<VehicleMileageLogResponse> CancelMileageLogAsync(Guid id, string? reason = null);
    Task DeleteMileageLogAsync(Guid id);
    Task<VehicleMileageLogResponse> GetMileageLogByIdAsync(Guid id);
    Task<IEnumerable<VehicleMileageLogResponse>> GetMileageLogsByVehicleAsync(Guid vehicleId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IEnumerable<VehicleMileageLogResponse>> GetActiveMileageLogsAsync();
    
    // Summary & Alerts
    Task<VehicleSummaryResponse> GetVehicleSummaryAsync(Guid vehicleId);
    Task<IEnumerable<VehicleAlertResponse>> GetAllAlertsAsync();
    Task<IEnumerable<VehicleAlertResponse>> GetAlertsByVehicleAsync(Guid vehicleId);
}
