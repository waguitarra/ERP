using Logistics.Application.DTOs.Vehicle;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Logistics.Infrastructure.Services;

public class VehicleManagementService : IVehicleManagementService
{
    private readonly LogisticsDbContext _context;

    public VehicleManagementService(LogisticsDbContext context)
    {
        _context = context;
    }

    // ===================== MAINTENANCE =====================

    public async Task<VehicleMaintenanceResponse> CreateMaintenanceAsync(CreateVehicleMaintenanceRequest request)
    {
        var vehicle = await _context.Vehicles.FindAsync(request.VehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var maintenance = new VehicleMaintenance(
            request.VehicleId,
            vehicle.CompanyId,
            request.Type,
            request.Description,
            request.MaintenanceDate,
            request.MileageAtMaintenance,
            request.LaborCost,
            request.PartsCost,
            request.ServiceProvider,
            request.InvoiceNumber,
            request.Notes
        );

        if (request.NextMaintenanceDate.HasValue)
            maintenance.SetNextMaintenance(request.NextMaintenanceDate.Value, request.NextMaintenanceMileage);

        _context.VehicleMaintenances.Add(maintenance);

        // Update vehicle last maintenance info
        vehicle.UpdateMaintenanceInfo(request.MaintenanceDate, request.MileageAtMaintenance, maintenance.TotalCost);

        await _context.SaveChangesAsync();
        return MapMaintenanceToResponse(maintenance, vehicle.LicensePlate);
    }

    public async Task<VehicleMaintenanceResponse> UpdateMaintenanceAsync(Guid id, UpdateVehicleMaintenanceRequest request)
    {
        var maintenance = await _context.VehicleMaintenances
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException("Manutenção não encontrada");

        maintenance.Update(
            request.Type,
            request.Description,
            request.MaintenanceDate,
            request.MileageAtMaintenance,
            request.LaborCost,
            request.PartsCost,
            request.ServiceProvider,
            request.ServiceProviderContact,
            request.InvoiceNumber,
            request.Notes
        );

        if (request.NextMaintenanceDate.HasValue)
            maintenance.SetNextMaintenance(request.NextMaintenanceDate.Value, request.NextMaintenanceMileage);

        await _context.SaveChangesAsync();
        return MapMaintenanceToResponse(maintenance, maintenance.Vehicle.LicensePlate);
    }

    public async Task DeleteMaintenanceAsync(Guid id)
    {
        var maintenance = await _context.VehicleMaintenances.FindAsync(id)
            ?? throw new KeyNotFoundException("Manutenção não encontrada");

        _context.VehicleMaintenances.Remove(maintenance);
        await _context.SaveChangesAsync();
    }

    public async Task<VehicleMaintenanceResponse> GetMaintenanceByIdAsync(Guid id)
    {
        var maintenance = await _context.VehicleMaintenances
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException("Manutenção não encontrada");

        return MapMaintenanceToResponse(maintenance, maintenance.Vehicle.LicensePlate);
    }

    public async Task<IEnumerable<VehicleMaintenanceResponse>> GetMaintenancesByVehicleAsync(Guid vehicleId)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var maintenances = await _context.VehicleMaintenances
            .Where(m => m.VehicleId == vehicleId)
            .OrderByDescending(m => m.MaintenanceDate)
            .ToListAsync();

        return maintenances.Select(m => MapMaintenanceToResponse(m, vehicle.LicensePlate));
    }

    // ===================== INSPECTION =====================

    public async Task<VehicleInspectionResponse> CreateInspectionAsync(CreateVehicleInspectionRequest request)
    {
        var vehicle = await _context.Vehicles.FindAsync(request.VehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var inspection = new VehicleInspection(
            request.VehicleId,
            vehicle.CompanyId,
            request.Type,
            request.InspectionDate,
            request.ExpiryDate,
            request.Result,
            request.MileageAtInspection,
            request.Cost,
            request.InspectionCenter,
            request.CertificateNumber,
            request.Observations
        );

        if (!string.IsNullOrEmpty(request.DefectsFound))
            inspection.SetDefects(request.DefectsFound);

        _context.VehicleInspections.Add(inspection);

        // Update vehicle inspection dates
        vehicle.UpdateInspectionDates(request.InspectionDate, request.ExpiryDate);

        await _context.SaveChangesAsync();
        return MapInspectionToResponse(inspection, vehicle.LicensePlate);
    }

    public async Task<VehicleInspectionResponse> UpdateInspectionAsync(Guid id, UpdateVehicleInspectionRequest request)
    {
        var inspection = await _context.VehicleInspections
            .Include(i => i.Vehicle)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new KeyNotFoundException("Inspeção não encontrada");

        inspection.Update(
            request.Type,
            request.InspectionDate,
            request.ExpiryDate,
            request.Result,
            request.MileageAtInspection,
            request.Cost,
            request.InspectionCenter,
            request.InspectorName,
            request.CertificateNumber,
            request.Observations,
            request.DefectsFound
        );

        await _context.SaveChangesAsync();
        return MapInspectionToResponse(inspection, inspection.Vehicle.LicensePlate);
    }

    public async Task DeleteInspectionAsync(Guid id)
    {
        var inspection = await _context.VehicleInspections.FindAsync(id)
            ?? throw new KeyNotFoundException("Inspeção não encontrada");

        _context.VehicleInspections.Remove(inspection);
        await _context.SaveChangesAsync();
    }

    public async Task<VehicleInspectionResponse> GetInspectionByIdAsync(Guid id)
    {
        var inspection = await _context.VehicleInspections
            .Include(i => i.Vehicle)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new KeyNotFoundException("Inspeção não encontrada");

        return MapInspectionToResponse(inspection, inspection.Vehicle.LicensePlate);
    }

    public async Task<IEnumerable<VehicleInspectionResponse>> GetInspectionsByVehicleAsync(Guid vehicleId)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var inspections = await _context.VehicleInspections
            .Where(i => i.VehicleId == vehicleId)
            .OrderByDescending(i => i.InspectionDate)
            .ToListAsync();

        return inspections.Select(i => MapInspectionToResponse(i, vehicle.LicensePlate));
    }

    public async Task<IEnumerable<VehicleInspectionResponse>> GetExpiringInspectionsAsync(int daysAhead = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);

        var inspections = await _context.VehicleInspections
            .Include(i => i.Vehicle)
            .Where(i => i.ExpiryDate <= cutoffDate && i.ExpiryDate >= DateTime.UtcNow)
            .OrderBy(i => i.ExpiryDate)
            .ToListAsync();

        return inspections.Select(i => MapInspectionToResponse(i, i.Vehicle.LicensePlate));
    }

    // ===================== DOCUMENT =====================

    public async Task<VehicleDocumentResponse> CreateDocumentAsync(CreateVehicleDocumentRequest request)
    {
        var vehicle = await _context.Vehicles.FindAsync(request.VehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var document = new VehicleDocument(
            request.VehicleId,
            vehicle.CompanyId,
            request.Type,
            request.DocumentNumber,
            request.IssueDate,
            request.ExpiryDate,
            request.Description,
            request.IssuingAuthority,
            request.Cost,
            request.AlertOnExpiry,
            request.AlertDaysBefore
        );

        _context.VehicleDocuments.Add(document);
        await _context.SaveChangesAsync();

        return MapDocumentToResponse(document, vehicle.LicensePlate);
    }

    public async Task<VehicleDocumentResponse> UpdateDocumentAsync(Guid id, UpdateVehicleDocumentRequest request)
    {
        var document = await _context.VehicleDocuments
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Documento não encontrado");

        document.Update(
            request.Type,
            request.DocumentNumber,
            request.IssueDate,
            request.ExpiryDate,
            request.Description,
            request.IssuingAuthority,
            request.Cost,
            request.AlertOnExpiry,
            request.AlertDaysBefore,
            request.Notes
        );

        await _context.SaveChangesAsync();
        return MapDocumentToResponse(document, document.Vehicle.LicensePlate);
    }

    public async Task DeleteDocumentAsync(Guid id)
    {
        var document = await _context.VehicleDocuments.FindAsync(id)
            ?? throw new KeyNotFoundException("Documento não encontrado");

        _context.VehicleDocuments.Remove(document);
        await _context.SaveChangesAsync();
    }

    public async Task<VehicleDocumentResponse> GetDocumentByIdAsync(Guid id)
    {
        var document = await _context.VehicleDocuments
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Documento não encontrado");

        return MapDocumentToResponse(document, document.Vehicle.LicensePlate);
    }

    public async Task<IEnumerable<VehicleDocumentResponse>> GetDocumentsByVehicleAsync(Guid vehicleId)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var documents = await _context.VehicleDocuments
            .Where(d => d.VehicleId == vehicleId)
            .OrderByDescending(d => d.IssueDate)
            .ToListAsync();

        return documents.Select(d => MapDocumentToResponse(d, vehicle.LicensePlate));
    }

    public async Task<IEnumerable<VehicleDocumentResponse>> GetExpiringDocumentsAsync(int daysAhead = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);

        var documents = await _context.VehicleDocuments
            .Include(d => d.Vehicle)
            .Where(d => d.ExpiryDate.HasValue && d.ExpiryDate <= cutoffDate && d.ExpiryDate >= DateTime.UtcNow)
            .OrderBy(d => d.ExpiryDate)
            .ToListAsync();

        return documents.Select(d => MapDocumentToResponse(d, d.Vehicle.LicensePlate));
    }

    public async Task<VehicleDocumentResponse> UploadDocumentFileAsync(Guid id, string fileName, string filePath, string fileType)
    {
        var document = await _context.VehicleDocuments
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Documento não encontrado");

        document.SetFile(fileName, filePath, fileType);
        await _context.SaveChangesAsync();

        return MapDocumentToResponse(document, document.Vehicle.LicensePlate);
    }

    // ===================== DAMAGE =====================

    public async Task<VehicleDamageResponse> CreateDamageAsync(CreateVehicleDamageRequest request)
    {
        var vehicle = await _context.Vehicles.FindAsync(request.VehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var damage = new VehicleDamage(
            request.VehicleId,
            vehicle.CompanyId,
            request.Title,
            request.Description,
            request.Type,
            request.Severity,
            request.OccurrenceDate,
            request.MileageAtOccurrence,
            request.DamageLocation,
            request.DriverId,
            request.DriverName
        );

        if (request.EstimatedRepairCost > 0 || request.IsThirdPartyFault)
        {
            damage.Update(
                request.Title,
                request.Description,
                request.Type,
                request.Severity,
                request.OccurrenceDate,
                request.MileageAtOccurrence,
                request.DamageLocation,
                request.EstimatedRepairCost,
                request.IsThirdPartyFault,
                request.ThirdPartyInfo
            );
        }

        _context.VehicleDamages.Add(damage);
        await _context.SaveChangesAsync();

        return MapDamageToResponse(damage, vehicle.LicensePlate);
    }

    public async Task<VehicleDamageResponse> UpdateDamageAsync(Guid id, UpdateVehicleDamageRequest request)
    {
        var damage = await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Avaria não encontrada");

        damage.Update(
            request.Title,
            request.Description,
            request.Type,
            request.Severity,
            request.OccurrenceDate,
            request.MileageAtOccurrence,
            request.DamageLocation,
            request.EstimatedRepairCost,
            request.IsThirdPartyFault,
            request.ThirdPartyInfo
        );

        await _context.SaveChangesAsync();
        return MapDamageToResponse(damage, damage.Vehicle.LicensePlate);
    }

    public async Task DeleteDamageAsync(Guid id)
    {
        var damage = await _context.VehicleDamages.FindAsync(id)
            ?? throw new KeyNotFoundException("Avaria não encontrada");

        _context.VehicleDamages.Remove(damage);
        await _context.SaveChangesAsync();
    }

    public async Task<VehicleDamageResponse> GetDamageByIdAsync(Guid id)
    {
        var damage = await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Avaria não encontrada");

        return MapDamageToResponse(damage, damage.Vehicle.LicensePlate);
    }

    public async Task<IEnumerable<VehicleDamageResponse>> GetAllDamagesAsync(Guid? companyId = null)
    {
        var query = _context.VehicleDamages
            .Include(d => d.Vehicle)
            .AsQueryable();

        if (companyId.HasValue)
            query = query.Where(d => d.CompanyId == companyId.Value);

        var damages = await query
            .OrderByDescending(d => d.OccurrenceDate)
            .ToListAsync();

        return damages.Select(d => MapDamageToResponse(d, d.Vehicle.LicensePlate));
    }

    public async Task<IEnumerable<VehicleDamageResponse>> GetDamagesByVehicleAsync(Guid vehicleId)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var damages = await _context.VehicleDamages
            .Where(d => d.VehicleId == vehicleId)
            .OrderByDescending(d => d.OccurrenceDate)
            .ToListAsync();

        return damages.Select(d => MapDamageToResponse(d, vehicle.LicensePlate));
    }

    public async Task<IEnumerable<VehicleDamageResponse>> GetPendingDamagesAsync()
    {
        var damages = await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .Where(d => d.Status != DamageStatus.Repaired && d.Status != DamageStatus.WriteOff)
            .OrderByDescending(d => d.Severity)
            .ThenByDescending(d => d.OccurrenceDate)
            .ToListAsync();

        return damages.Select(d => MapDamageToResponse(d, d.Vehicle.LicensePlate));
    }

    public async Task<VehicleDamageResponse> RepairDamageAsync(Guid id, RepairVehicleDamageRequest request)
    {
        var damage = await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Avaria não encontrada");

        damage.SetRepairInfo(
            request.RepairShop,
            request.ActualRepairCost,
            request.RepairedDate,
            request.RepairNotes
        );

        await _context.SaveChangesAsync();
        return MapDamageToResponse(damage, damage.Vehicle.LicensePlate);
    }

    public async Task<VehicleDamageResponse> SetInsuranceClaimAsync(Guid id, VehicleDamageInsuranceRequest request)
    {
        var damage = await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Avaria não encontrada");

        damage.SetInsuranceClaim(request.ClaimNumber, request.Reimbursement);

        await _context.SaveChangesAsync();
        return MapDamageToResponse(damage, damage.Vehicle.LicensePlate);
    }

    public async Task<VehicleDamageResponse> UpdateDamagePhotosAsync(Guid id, VehicleDamagePhotosRequest request)
    {
        var damage = await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Avaria não encontrada");

        var photosJson = JsonSerializer.Serialize(request.PhotoUrls);
        damage.UpdatePhotos(photosJson);

        await _context.SaveChangesAsync();
        return MapDamageToResponse(damage, damage.Vehicle.LicensePlate);
    }

    public async Task<VehicleDamageResponse> UpdateDamageStatusAsync(Guid id, DamageStatus status)
    {
        var damage = await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new KeyNotFoundException("Avaria não encontrada");

        damage.UpdateStatus(status);

        await _context.SaveChangesAsync();
        return MapDamageToResponse(damage, damage.Vehicle.LicensePlate);
    }

    // ===================== MILEAGE LOG =====================

    public async Task<VehicleMileageLogResponse> CreateMileageLogAsync(CreateVehicleMileageLogRequest request)
    {
        var vehicle = await _context.Vehicles.FindAsync(request.VehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var mileageLog = new VehicleMileageLog(
            request.VehicleId,
            vehicle.CompanyId,
            request.Type,
            request.StartMileage,
            request.StartDateTime,
            request.StartLatitude,
            request.StartLongitude,
            request.StartAddress,
            request.DriverId,
            request.DriverName,
            request.ShipmentId,
            request.ShipmentNumber,
            request.Purpose
        );

        _context.VehicleMileageLogs.Add(mileageLog);
        await _context.SaveChangesAsync();

        return MapMileageLogToResponse(mileageLog, vehicle.LicensePlate);
    }

    public async Task<VehicleMileageLogResponse> CompleteMileageLogAsync(Guid id, CompleteMileageLogRequest request)
    {
        var mileageLog = await _context.VehicleMileageLogs
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException("Registro de quilometragem não encontrado");

        mileageLog.CompleteTrip(
            request.EndMileage,
            request.EndDateTime,
            request.EndLatitude,
            request.EndLongitude,
            request.EndAddress,
            request.RoutePolyline
        );

        if (request.FuelConsumed.HasValue || request.FuelCost.HasValue)
            mileageLog.SetFuelInfo(request.FuelConsumed ?? 0, request.FuelCost ?? 0);

        if (request.TollCost.HasValue || request.ParkingCost.HasValue || request.OtherCosts.HasValue)
            mileageLog.SetCosts(request.TollCost, request.ParkingCost, request.OtherCosts);

        // Update vehicle mileage
        mileageLog.Vehicle.UpdateMileage(request.EndMileage);

        await _context.SaveChangesAsync();
        return MapMileageLogToResponse(mileageLog, mileageLog.Vehicle.LicensePlate);
    }

    public async Task<VehicleMileageLogResponse> UpdateMileageLogCostsAsync(Guid id, UpdateMileageLogCostsRequest request)
    {
        var mileageLog = await _context.VehicleMileageLogs
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException("Registro de quilometragem não encontrado");

        if (request.FuelConsumed.HasValue || request.FuelCost.HasValue)
            mileageLog.SetFuelInfo(request.FuelConsumed ?? 0, request.FuelCost ?? 0);

        mileageLog.SetCosts(request.TollCost, request.ParkingCost, request.OtherCosts);

        await _context.SaveChangesAsync();
        return MapMileageLogToResponse(mileageLog, mileageLog.Vehicle.LicensePlate);
    }

    public async Task<VehicleMileageLogResponse> CancelMileageLogAsync(Guid id, string? reason = null)
    {
        var mileageLog = await _context.VehicleMileageLogs
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException("Registro de quilometragem não encontrado");

        mileageLog.Cancel(reason);

        await _context.SaveChangesAsync();
        return MapMileageLogToResponse(mileageLog, mileageLog.Vehicle.LicensePlate);
    }

    public async Task DeleteMileageLogAsync(Guid id)
    {
        var mileageLog = await _context.VehicleMileageLogs.FindAsync(id)
            ?? throw new KeyNotFoundException("Registro de quilometragem não encontrado");

        _context.VehicleMileageLogs.Remove(mileageLog);
        await _context.SaveChangesAsync();
    }

    public async Task<VehicleMileageLogResponse> GetMileageLogByIdAsync(Guid id)
    {
        var mileageLog = await _context.VehicleMileageLogs
            .Include(m => m.Vehicle)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException("Registro de quilometragem não encontrado");

        return MapMileageLogToResponse(mileageLog, mileageLog.Vehicle.LicensePlate);
    }

    public async Task<IEnumerable<VehicleMileageLogResponse>> GetMileageLogsByVehicleAsync(Guid vehicleId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var query = _context.VehicleMileageLogs
            .Where(m => m.VehicleId == vehicleId);

        if (startDate.HasValue)
            query = query.Where(m => m.StartDateTime >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(m => m.StartDateTime <= endDate.Value);

        var logs = await query
            .OrderByDescending(m => m.StartDateTime)
            .ToListAsync();

        return logs.Select(m => MapMileageLogToResponse(m, vehicle.LicensePlate));
    }

    public async Task<IEnumerable<VehicleMileageLogResponse>> GetActiveMileageLogsAsync()
    {
        var logs = await _context.VehicleMileageLogs
            .Include(m => m.Vehicle)
            .Where(m => m.Status == MileageLogStatus.InProgress)
            .OrderByDescending(m => m.StartDateTime)
            .ToListAsync();

        return logs.Select(m => MapMileageLogToResponse(m, m.Vehicle.LicensePlate));
    }

    // ===================== SUMMARY & ALERTS =====================

    public async Task<VehicleSummaryResponse> GetVehicleSummaryAsync(Guid vehicleId)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Maintenances)
            .Include(v => v.Damages)
            .Include(v => v.Documents)
            .Include(v => v.Inspections)
            .FirstOrDefaultAsync(v => v.Id == vehicleId)
            ?? throw new KeyNotFoundException("Veículo não encontrado");

        var deliveryCount = await _context.OutboundShipments
            .CountAsync(s => s.VehicleId == vehicleId);

        var alerts = await GetAlertsByVehicleAsync(vehicleId);

        return new VehicleSummaryResponse
        {
            VehicleId = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Model = vehicle.Model,
            CurrentMileage = vehicle.CurrentMileage,
            TotalMaintenanceCost = vehicle.TotalMaintenanceCost,
            TotalDamageCost = vehicle.Damages.Sum(d => d.ActualRepairCost),
            TotalDeliveries = deliveryCount,
            PendingDamages = vehicle.Damages.Count(d => d.Status != DamageStatus.Repaired && d.Status != DamageStatus.WriteOff),
            ExpiringDocuments = vehicle.Documents.Count(d => d.ExpiryDate.HasValue && d.ExpiryDate <= DateTime.UtcNow.AddDays(30)),
            ExpiringInspections = vehicle.Inspections.Count(i => i.ExpiryDate <= DateTime.UtcNow.AddDays(30)),
            NextMaintenanceDate = vehicle.Maintenances
                .Where(m => m.NextMaintenanceDate.HasValue && m.NextMaintenanceDate > DateTime.UtcNow)
                .OrderBy(m => m.NextMaintenanceDate)
                .Select(m => m.NextMaintenanceDate)
                .FirstOrDefault(),
            NextInspectionDate = vehicle.NextInspectionDate,
            Alerts = alerts.ToList()
        };
    }

    public async Task<IEnumerable<VehicleAlertResponse>> GetAllAlertsAsync()
    {
        var alerts = new List<VehicleAlertResponse>();

        // Expiring inspections
        var expiringInspections = await _context.VehicleInspections
            .Include(i => i.Vehicle)
            .Where(i => i.ExpiryDate <= DateTime.UtcNow.AddDays(30))
            .ToListAsync();

        foreach (var inspection in expiringInspections)
        {
            var daysUntil = (inspection.ExpiryDate - DateTime.UtcNow).Days;
            alerts.Add(new VehicleAlertResponse
            {
                VehicleId = inspection.VehicleId,
                VehicleLicensePlate = inspection.Vehicle.LicensePlate,
                AlertType = "InspectionExpiring",
                Title = $"Inspeção vencendo: {inspection.Type}",
                Description = daysUntil < 0 
                    ? $"Inspeção vencida há {Math.Abs(daysUntil)} dias" 
                    : $"Inspeção vence em {daysUntil} dias",
                DueDate = inspection.ExpiryDate,
                DaysUntilDue = daysUntil,
                Severity = daysUntil < 0 ? "Critical" : daysUntil < 7 ? "High" : "Medium"
            });
        }

        // Expiring documents
        var expiringDocuments = await _context.VehicleDocuments
            .Include(d => d.Vehicle)
            .Where(d => d.AlertOnExpiry && d.ExpiryDate.HasValue && d.ExpiryDate <= DateTime.UtcNow.AddDays(30))
            .ToListAsync();

        foreach (var document in expiringDocuments)
        {
            var daysUntil = (document.ExpiryDate!.Value - DateTime.UtcNow).Days;
            alerts.Add(new VehicleAlertResponse
            {
                VehicleId = document.VehicleId,
                VehicleLicensePlate = document.Vehicle.LicensePlate,
                AlertType = "DocumentExpiring",
                Title = $"Documento vencendo: {document.Type}",
                Description = daysUntil < 0 
                    ? $"Documento vencido há {Math.Abs(daysUntil)} dias" 
                    : $"Documento vence em {daysUntil} dias",
                DueDate = document.ExpiryDate,
                DaysUntilDue = daysUntil,
                Severity = daysUntil < 0 ? "Critical" : daysUntil < 7 ? "High" : "Medium"
            });
        }

        // Pending damages
        var pendingDamages = await _context.VehicleDamages
            .Include(d => d.Vehicle)
            .Where(d => d.Status != DamageStatus.Repaired && d.Status != DamageStatus.WriteOff)
            .ToListAsync();

        foreach (var damage in pendingDamages)
        {
            alerts.Add(new VehicleAlertResponse
            {
                VehicleId = damage.VehicleId,
                VehicleLicensePlate = damage.Vehicle.LicensePlate,
                AlertType = "PendingDamage",
                Title = $"Avaria pendente: {damage.Title}",
                Description = $"Severidade: {damage.Severity} - Status: {damage.Status}",
                DueDate = null,
                DaysUntilDue = null,
                Severity = damage.Severity == DamageSeverity.Critical ? "Critical" : 
                           damage.Severity == DamageSeverity.Major ? "High" : "Medium"
            });
        }

        return alerts.OrderByDescending(a => a.Severity == "Critical")
            .ThenByDescending(a => a.Severity == "High")
            .ThenBy(a => a.DaysUntilDue);
    }

    public async Task<IEnumerable<VehicleAlertResponse>> GetAlertsByVehicleAsync(Guid vehicleId)
    {
        var allAlerts = await GetAllAlertsAsync();
        return allAlerts.Where(a => a.VehicleId == vehicleId);
    }

    // ===================== MAPPERS =====================

    private static VehicleMaintenanceResponse MapMaintenanceToResponse(VehicleMaintenance m, string licensePlate)
    {
        return new VehicleMaintenanceResponse
        {
            Id = m.Id,
            VehicleId = m.VehicleId,
            VehicleLicensePlate = licensePlate,
            Type = m.Type,
            TypeName = m.Type.ToString(),
            Description = m.Description,
            MaintenanceDate = m.MaintenanceDate,
            NextMaintenanceDate = m.NextMaintenanceDate,
            MileageAtMaintenance = m.MileageAtMaintenance,
            NextMaintenanceMileage = m.NextMaintenanceMileage,
            LaborCost = m.LaborCost,
            PartsCost = m.PartsCost,
            TotalCost = m.TotalCost,
            ServiceProvider = m.ServiceProvider,
            ServiceProviderContact = m.ServiceProviderContact,
            InvoiceNumber = m.InvoiceNumber,
            Notes = m.Notes,
            CreatedAt = m.CreatedAt
        };
    }

    private static VehicleInspectionResponse MapInspectionToResponse(VehicleInspection i, string licensePlate)
    {
        return new VehicleInspectionResponse
        {
            Id = i.Id,
            VehicleId = i.VehicleId,
            VehicleLicensePlate = licensePlate,
            Type = i.Type,
            TypeName = i.Type.ToString(),
            InspectionDate = i.InspectionDate,
            ExpiryDate = i.ExpiryDate,
            Result = i.Result,
            ResultName = i.Result.ToString(),
            InspectionCenter = i.InspectionCenter,
            InspectorName = i.InspectorName,
            CertificateNumber = i.CertificateNumber,
            MileageAtInspection = i.MileageAtInspection,
            Cost = i.Cost,
            Observations = i.Observations,
            DefectsFound = i.DefectsFound,
            IsExpired = i.ExpiryDate < DateTime.UtcNow,
            IsExpiringSoon = i.ExpiryDate <= DateTime.UtcNow.AddDays(30),
            CreatedAt = i.CreatedAt
        };
    }

    private static VehicleDocumentResponse MapDocumentToResponse(VehicleDocument d, string licensePlate)
    {
        return new VehicleDocumentResponse
        {
            Id = d.Id,
            VehicleId = d.VehicleId,
            VehicleLicensePlate = licensePlate,
            Type = d.Type,
            TypeName = d.Type.ToString(),
            DocumentNumber = d.DocumentNumber,
            Description = d.Description,
            IssueDate = d.IssueDate,
            ExpiryDate = d.ExpiryDate,
            IssuingAuthority = d.IssuingAuthority,
            FileName = d.FileName,
            FilePath = d.FilePath,
            FileType = d.FileType,
            Cost = d.Cost,
            AlertOnExpiry = d.AlertOnExpiry,
            AlertDaysBefore = d.AlertDaysBefore,
            Notes = d.Notes,
            IsExpired = d.ExpiryDate.HasValue && d.ExpiryDate < DateTime.UtcNow,
            IsExpiringSoon = d.ExpiryDate.HasValue && d.ExpiryDate <= DateTime.UtcNow.AddDays(30),
            CreatedAt = d.CreatedAt
        };
    }

    private static VehicleDamageResponse MapDamageToResponse(VehicleDamage d, string licensePlate)
    {
        var photoUrls = new List<string>();
        if (!string.IsNullOrEmpty(d.PhotoUrls))
        {
            try
            {
                photoUrls = JsonSerializer.Deserialize<List<string>>(d.PhotoUrls) ?? new List<string>();
            }
            catch { }
        }

        return new VehicleDamageResponse
        {
            Id = d.Id,
            VehicleId = d.VehicleId,
            VehicleLicensePlate = licensePlate,
            Title = d.Title,
            Description = d.Description,
            Type = d.Type,
            TypeName = d.Type.ToString(),
            Severity = d.Severity,
            SeverityName = d.Severity.ToString(),
            DamageLocation = d.DamageLocation,
            OccurrenceDate = d.OccurrenceDate,
            ReportedDate = d.ReportedDate,
            RepairedDate = d.RepairedDate,
            MileageAtOccurrence = d.MileageAtOccurrence,
            Status = d.Status,
            StatusName = d.Status.ToString(),
            EstimatedRepairCost = d.EstimatedRepairCost,
            ActualRepairCost = d.ActualRepairCost,
            DriverId = d.DriverId,
            DriverName = d.DriverName,
            IsThirdPartyFault = d.IsThirdPartyFault,
            ThirdPartyInfo = d.ThirdPartyInfo,
            InsuranceClaim = d.InsuranceClaim,
            InsuranceClaimNumber = d.InsuranceClaimNumber,
            InsuranceReimbursement = d.InsuranceReimbursement,
            RepairShop = d.RepairShop,
            RepairNotes = d.RepairNotes,
            PhotoUrls = photoUrls,
            Notes = d.Notes,
            CreatedAt = d.CreatedAt
        };
    }

    private static VehicleMileageLogResponse MapMileageLogToResponse(VehicleMileageLog m, string licensePlate)
    {
        return new VehicleMileageLogResponse
        {
            Id = m.Id,
            VehicleId = m.VehicleId,
            VehicleLicensePlate = licensePlate,
            Type = m.Type,
            TypeName = m.Type.ToString(),
            StartMileage = m.StartMileage,
            EndMileage = m.EndMileage,
            Distance = m.Distance,
            StartLatitude = m.StartLatitude,
            StartLongitude = m.StartLongitude,
            StartAddress = m.StartAddress,
            EndLatitude = m.EndLatitude,
            EndLongitude = m.EndLongitude,
            EndAddress = m.EndAddress,
            StartDateTime = m.StartDateTime,
            EndDateTime = m.EndDateTime,
            Duration = m.Duration,
            DriverId = m.DriverId,
            DriverName = m.DriverName,
            ShipmentId = m.ShipmentId,
            ShipmentNumber = m.ShipmentNumber,
            FuelConsumed = m.FuelConsumed,
            FuelCost = m.FuelCost,
            FuelEfficiency = m.FuelEfficiency,
            Status = m.Status,
            StatusName = m.Status.ToString(),
            TollCost = m.TollCost,
            ParkingCost = m.ParkingCost,
            OtherCosts = m.OtherCosts,
            TotalCost = m.TotalCost,
            Purpose = m.Purpose,
            Notes = m.Notes,
            RoutePolyline = m.RoutePolyline,
            CreatedAt = m.CreatedAt
        };
    }
}
