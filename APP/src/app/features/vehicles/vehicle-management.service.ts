import { Injectable, inject } from '@angular/core';
import { Observable, from, map } from 'rxjs';
import { ApiService } from '@core/services/api.service';
import {
  VehicleMaintenance, VehicleMaintenanceResponse, VehicleMaintenanceListResponse,
  CreateMaintenanceRequest,
  VehicleInspection, VehicleInspectionResponse, VehicleInspectionListResponse,
  CreateInspectionRequest,
  VehicleDocument, VehicleDocumentResponse, VehicleDocumentListResponse,
  CreateDocumentRequest,
  VehicleDamage, VehicleDamageResponse, VehicleDamageListResponse,
  CreateDamageRequest, RepairDamageRequest, DamageInsuranceRequest, DamagePhotosRequest,
  DamageStatus,
  VehicleMileageLog, VehicleMileageLogResponse, VehicleMileageLogListResponse,
  CreateMileageLogRequest, CompleteMileageLogRequest, UpdateMileageLogCostsRequest,
  VehicleAlert, VehicleAlertListResponse,
  VehicleSummary, VehicleSummaryResponse
} from '@core/models/vehicle-management.model';

// Re-export entity types for convenience (these are used directly in page components)
export type { 
  VehicleMaintenance as VehicleMaintenanceResponse, 
  VehicleInspection as VehicleInspectionResponse, 
  VehicleDocument as VehicleDocumentResponse, 
  VehicleDamage as VehicleDamageResponse, 
  VehicleMileageLog as VehicleMileageLogResponse,
  VehicleAlert as VehicleAlertResponse
} from '@core/models/vehicle-management.model';

@Injectable({
  providedIn: 'root'
})
export class VehicleManagementService {
  private readonly api = inject(ApiService);
  private readonly baseEndpoint = '/vehicles';

  private managementEndpoint(vehicleId: string): string {
    return `${this.baseEndpoint}/${vehicleId}/management`;
  }

  // ============ MAINTENANCE ============

  getMaintenances(vehicleId: string): Promise<VehicleMaintenanceListResponse> {
    return this.api.get<VehicleMaintenanceListResponse>(
      `${this.managementEndpoint(vehicleId)}/maintenances`
    );
  }

  getMaintenance(vehicleId: string, id: string): Promise<VehicleMaintenanceResponse> {
    return this.api.get<VehicleMaintenanceResponse>(
      `${this.managementEndpoint(vehicleId)}/maintenances/${id}`
    );
  }

  createMaintenance(vehicleId: string, data: CreateMaintenanceRequest): Promise<VehicleMaintenanceResponse> {
    return this.api.post<VehicleMaintenanceResponse>(
      `${this.managementEndpoint(vehicleId)}/maintenances`,
      data
    );
  }

  updateMaintenance(vehicleId: string, id: string, data: CreateMaintenanceRequest): Promise<VehicleMaintenanceResponse> {
    return this.api.put<VehicleMaintenanceResponse>(
      `${this.managementEndpoint(vehicleId)}/maintenances/${id}`,
      data
    );
  }

  deleteMaintenance(vehicleId: string, id: string): Promise<void> {
    return this.api.delete<void>(
      `${this.managementEndpoint(vehicleId)}/maintenances/${id}`
    );
  }

  // ============ INSPECTION ============

  getInspections(vehicleId: string): Promise<VehicleInspectionListResponse> {
    return this.api.get<VehicleInspectionListResponse>(
      `${this.managementEndpoint(vehicleId)}/inspections`
    );
  }

  getInspection(vehicleId: string, id: string): Promise<VehicleInspectionResponse> {
    return this.api.get<VehicleInspectionResponse>(
      `${this.managementEndpoint(vehicleId)}/inspections/${id}`
    );
  }

  createInspection(vehicleId: string, data: CreateInspectionRequest): Promise<VehicleInspectionResponse> {
    return this.api.post<VehicleInspectionResponse>(
      `${this.managementEndpoint(vehicleId)}/inspections`,
      data
    );
  }

  updateInspection(vehicleId: string, id: string, data: CreateInspectionRequest): Promise<VehicleInspectionResponse> {
    return this.api.put<VehicleInspectionResponse>(
      `${this.managementEndpoint(vehicleId)}/inspections/${id}`,
      data
    );
  }

  deleteInspection(vehicleId: string, id: string): Promise<void> {
    return this.api.delete<void>(
      `${this.managementEndpoint(vehicleId)}/inspections/${id}`
    );
  }

  // ============ DOCUMENT ============

  getDocuments(vehicleId: string): Promise<VehicleDocumentListResponse> {
    return this.api.get<VehicleDocumentListResponse>(
      `${this.managementEndpoint(vehicleId)}/documents`
    );
  }

  getDocument(vehicleId: string, id: string): Promise<VehicleDocumentResponse> {
    return this.api.get<VehicleDocumentResponse>(
      `${this.managementEndpoint(vehicleId)}/documents/${id}`
    );
  }

  createDocument(vehicleId: string, data: CreateDocumentRequest): Promise<VehicleDocumentResponse> {
    return this.api.post<VehicleDocumentResponse>(
      `${this.managementEndpoint(vehicleId)}/documents`,
      data
    );
  }

  updateDocument(vehicleId: string, id: string, data: CreateDocumentRequest): Promise<VehicleDocumentResponse> {
    return this.api.put<VehicleDocumentResponse>(
      `${this.managementEndpoint(vehicleId)}/documents/${id}`,
      data
    );
  }

  deleteDocument(vehicleId: string, id: string): Promise<void> {
    return this.api.delete<void>(
      `${this.managementEndpoint(vehicleId)}/documents/${id}`
    );
  }

  // ============ DAMAGE ============

  getAllDamages(companyId?: string): Promise<VehicleDamageListResponse> {
    const params = companyId ? { companyId } : undefined;
    return this.api.get<VehicleDamageListResponse>(
      `/vehicles/damages`,
      params
    );
  }

  getDamages(vehicleId: string): Promise<VehicleDamageListResponse> {
    return this.api.get<VehicleDamageListResponse>(
      `${this.managementEndpoint(vehicleId)}/damages`
    );
  }

  getDamage(vehicleId: string, id: string): Promise<VehicleDamageResponse> {
    return this.api.get<VehicleDamageResponse>(
      `${this.managementEndpoint(vehicleId)}/damages/${id}`
    );
  }

  createDamage(vehicleId: string, data: CreateDamageRequest): Promise<VehicleDamageResponse> {
    return this.api.post<VehicleDamageResponse>(
      `${this.managementEndpoint(vehicleId)}/damages`,
      data
    );
  }

  updateDamage(vehicleId: string, id: string, data: CreateDamageRequest): Promise<VehicleDamageResponse> {
    return this.api.put<VehicleDamageResponse>(
      `${this.managementEndpoint(vehicleId)}/damages/${id}`,
      data
    );
  }

  repairDamage(vehicleId: string, id: string, data: RepairDamageRequest): Promise<VehicleDamageResponse> {
    return this.api.post<VehicleDamageResponse>(
      `${this.managementEndpoint(vehicleId)}/damages/${id}/repair`,
      data
    );
  }

  setDamageInsurance(vehicleId: string, id: string, data: DamageInsuranceRequest): Promise<VehicleDamageResponse> {
    return this.api.post<VehicleDamageResponse>(
      `${this.managementEndpoint(vehicleId)}/damages/${id}/insurance`,
      data
    );
  }

  updateDamagePhotos(vehicleId: string, id: string, data: DamagePhotosRequest): Promise<VehicleDamageResponse> {
    return this.api.post<VehicleDamageResponse>(
      `${this.managementEndpoint(vehicleId)}/damages/${id}/photos`,
      data
    );
  }

  updateDamageStatus(vehicleId: string, id: string, status: DamageStatus): Promise<VehicleDamageResponse> {
    return this.api.patch<VehicleDamageResponse>(
      `${this.managementEndpoint(vehicleId)}/damages/${id}/status`,
      { status }
    );
  }

  deleteDamage(vehicleId: string, id: string): Promise<void> {
    return this.api.delete<void>(
      `${this.managementEndpoint(vehicleId)}/damages/${id}`
    );
  }

  // ============ MILEAGE LOG ============

  getMileageLogs(vehicleId: string, startDate?: string, endDate?: string): Promise<VehicleMileageLogListResponse> {
    const params: any = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    return this.api.get<VehicleMileageLogListResponse>(
      `${this.managementEndpoint(vehicleId)}/mileage-logs`,
      params
    );
  }

  getMileageLog(vehicleId: string, id: string): Promise<VehicleMileageLogResponse> {
    return this.api.get<VehicleMileageLogResponse>(
      `${this.managementEndpoint(vehicleId)}/mileage-logs/${id}`
    );
  }

  createMileageLog(vehicleId: string, data: CreateMileageLogRequest): Promise<VehicleMileageLogResponse> {
    return this.api.post<VehicleMileageLogResponse>(
      `${this.managementEndpoint(vehicleId)}/mileage-logs`,
      data
    );
  }

  updateMileageLog(vehicleId: string, id: string, data: CreateMileageLogRequest): Promise<VehicleMileageLogResponse> {
    return this.api.put<VehicleMileageLogResponse>(
      `${this.managementEndpoint(vehicleId)}/mileage-logs/${id}`,
      data
    );
  }

  completeMileageLog(vehicleId: string, id: string, data: CompleteMileageLogRequest): Promise<VehicleMileageLogResponse> {
    return this.api.post<VehicleMileageLogResponse>(
      `${this.managementEndpoint(vehicleId)}/mileage-logs/${id}/complete`,
      data
    );
  }

  updateMileageLogCosts(vehicleId: string, id: string, data: UpdateMileageLogCostsRequest): Promise<VehicleMileageLogResponse> {
    return this.api.put<VehicleMileageLogResponse>(
      `${this.managementEndpoint(vehicleId)}/mileage-logs/${id}/costs`,
      data
    );
  }

  cancelMileageLog(vehicleId: string, id: string, reason?: string): Promise<VehicleMileageLogResponse> {
    return this.api.post<VehicleMileageLogResponse>(
      `${this.managementEndpoint(vehicleId)}/mileage-logs/${id}/cancel`,
      { reason }
    );
  }

  deleteMileageLog(vehicleId: string, id: string): Promise<void> {
    return this.api.delete<void>(
      `${this.managementEndpoint(vehicleId)}/mileage-logs/${id}`
    );
  }

  // ============ SUMMARY & ALERTS ============

  getVehicleSummary(vehicleId: string): Promise<VehicleSummaryResponse> {
    return this.api.get<VehicleSummaryResponse>(
      `${this.managementEndpoint(vehicleId)}/summary`
    );
  }

  getVehicleAlerts(vehicleId: string): Promise<VehicleAlertListResponse> {
    return this.api.get<VehicleAlertListResponse>(
      `${this.managementEndpoint(vehicleId)}/alerts`
    );
  }

  // ============ GLOBAL ALERTS ============

  getAllAlerts(): Promise<VehicleAlertListResponse> {
    return this.api.get<VehicleAlertListResponse>('/vehicle-alerts');
  }

  getExpiringInspections(daysAhead: number = 30): Promise<VehicleInspectionListResponse> {
    return this.api.get<VehicleInspectionListResponse>(
      '/vehicle-alerts/expiring-inspections',
      { daysAhead }
    );
  }

  getExpiringDocuments(daysAhead: number = 30): Promise<VehicleDocumentListResponse> {
    return this.api.get<VehicleDocumentListResponse>(
      '/vehicle-alerts/expiring-documents',
      { daysAhead }
    );
  }

  getPendingDamages(): Promise<VehicleDamageListResponse> {
    return this.api.get<VehicleDamageListResponse>('/vehicle-alerts/pending-damages');
  }

  getActiveTrips(): Promise<VehicleMileageLogListResponse> {
    return this.api.get<VehicleMileageLogListResponse>('/vehicle-alerts/active-trips');
  }

  // ============ OBSERVABLE METHODS FOR LISTING ============

  getMaintenancesByVehicle(vehicleId: string): Observable<VehicleMaintenance[]> {
    return from(this.getMaintenances(vehicleId)).pipe(
      map(response => response.data || [])
    );
  }

  getInspectionsByVehicle(vehicleId: string): Observable<VehicleInspection[]> {
    return from(this.getInspections(vehicleId)).pipe(
      map(response => response.data || [])
    );
  }

  getDocumentsByVehicle(vehicleId: string): Observable<VehicleDocument[]> {
    return from(this.getDocuments(vehicleId)).pipe(
      map(response => response.data || [])
    );
  }

  getDamagesByVehicle(vehicleId: string): Observable<VehicleDamage[]> {
    return from(this.getDamages(vehicleId)).pipe(
      map(response => response.data || [])
    );
  }

  getMileageLogsByVehicle(vehicleId: string): Observable<VehicleMileageLog[]> {
    return from(this.getMileageLogs(vehicleId)).pipe(
      map(response => response.data || [])
    );
  }
}
