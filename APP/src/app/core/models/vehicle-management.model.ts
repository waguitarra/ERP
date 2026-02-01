// ============ MAINTENANCE ============

export interface VehicleMaintenance {
  id: string;
  vehicleId: string;
  vehicleLicensePlate: string;
  type: MaintenanceType;
  typeName: string;
  description: string;
  maintenanceDate: string;
  nextMaintenanceDate?: string;
  mileageAtMaintenance: number;
  nextMaintenanceMileage?: number;
  laborCost: number;
  partsCost: number;
  totalCost: number;
  serviceProvider?: string;
  serviceProviderContact?: string;
  invoiceNumber?: string;
  notes?: string;
  createdAt: string;
}

export interface CreateMaintenanceRequest {
  vehicleId?: string;
  type: MaintenanceType;
  description: string;
  maintenanceDate: string;
  nextMaintenanceDate?: string;
  mileageAtMaintenance: number;
  nextMaintenanceMileage?: number;
  laborCost: number;
  partsCost: number;
  serviceProvider?: string;
  serviceProviderContact?: string;
  invoiceNumber?: string;
  notes?: string;
}

export enum MaintenanceType {
  Preventive = 0,
  Corrective = 1,
  OilChange = 2,
  TireChange = 3,
  BrakeService = 4,
  EngineRepair = 5,
  Transmission = 6,
  Electrical = 7,
  BodyWork = 8,
  AirConditioning = 9,
  Other = 99
}

// ============ INSPECTION ============

export interface VehicleInspection {
  id: string;
  vehicleId: string;
  vehicleLicensePlate: string;
  type: InspectionType;
  typeName: string;
  inspectionDate: string;
  expiryDate: string;
  result: InspectionResult;
  resultName: string;
  inspectionCenter?: string;
  inspectorName?: string;
  certificateNumber?: string;
  mileageAtInspection: number;
  cost: number;
  observations?: string;
  defectsFound?: string;
  isExpired: boolean;
  isExpiringSoon: boolean;
  createdAt: string;
}

export interface CreateInspectionRequest {
  vehicleId?: string;
  type: InspectionType;
  inspectionDate: string;
  expiryDate: string;
  result: InspectionResult;
  inspectionCenter?: string;
  inspectorName?: string;
  certificateNumber?: string;
  mileageAtInspection: number;
  cost: number;
  observations?: string;
  defectsFound?: string;
}

export enum InspectionType {
  ITV = 0,
  MOT = 1,
  TUV = 2,
  CT = 3,
  Revisione = 4,
  APK = 5,
  DETRAN = 6,
  SafetyInspection = 7,
  EmissionsTest = 8,
  Other = 99
}

export enum InspectionResult {
  Approved = 0,
  ApprovedWithDefects = 1,
  Rejected = 2,
  Pending = 3
}

// ============ DOCUMENT ============

export interface VehicleDocument {
  id: string;
  vehicleId: string;
  vehicleLicensePlate: string;
  type: VehicleDocumentType;
  typeName: string;
  documentNumber: string;
  description?: string;
  issueDate: string;
  expiryDate?: string;
  issuingAuthority?: string;
  fileName?: string;
  filePath?: string;
  fileType?: string;
  cost?: number;
  alertOnExpiry: boolean;
  alertDaysBefore?: number;
  notes?: string;
  isExpired: boolean;
  isExpiringSoon: boolean;
  createdAt: string;
}

export interface CreateDocumentRequest {
  vehicleId?: string;
  type: VehicleDocumentType;
  documentNumber: string;
  description?: string;
  issueDate: string;
  expiryDate?: string;
  issuingAuthority?: string;
  cost?: number;
  alertOnExpiry?: boolean;
  alertDaysBefore?: number;
  notes?: string;
}

export enum VehicleDocumentType {
  RegistrationCertificate = 0,
  OwnershipTitle = 1,
  Insurance = 10,
  ThirdPartyInsurance = 11,
  CargoInsurance = 12,
  OperatingLicense = 20,
  TransportLicense = 21,
  HazmatLicense = 22,
  RoadTax = 30,
  TollTag = 31,
  TachographCertificate = 40,
  SpeedometerCertificate = 41,
  PurchaseInvoice = 50,
  Warranty = 51,
  Manual = 52,
  Other = 99
}

// ============ DAMAGE ============

export interface VehicleDamage {
  id: string;
  vehicleId: string;
  vehicleLicensePlate: string;
  title: string;
  description: string;
  type: DamageType;
  typeName: string;
  severity: DamageSeverity;
  severityName: string;
  damageLocation?: string;
  occurrenceDate: string;
  reportedDate?: string;
  repairedDate?: string;
  mileageAtOccurrence: number;
  status: DamageStatus;
  statusName: string;
  estimatedRepairCost: number;
  actualRepairCost: number;
  driverId?: string;
  driverName?: string;
  isThirdPartyFault: boolean;
  thirdPartyInfo?: string;
  insuranceClaim: boolean;
  insuranceClaimNumber?: string;
  insuranceReimbursement?: number;
  repairShop?: string;
  repairNotes?: string;
  photoUrls: string[];
  notes?: string;
  createdAt: string;
}

export interface CreateDamageRequest {
  vehicleId?: string;
  title: string;
  description: string;
  type: DamageType;
  severity: DamageSeverity;
  damageLocation?: string;
  occurrenceDate: string;
  mileageAtOccurrence: number;
  estimatedRepairCost?: number;
  driverId?: string;
  driverName?: string;
  isThirdPartyFault?: boolean;
  thirdPartyInfo?: string;
  notes?: string;
}

export interface RepairDamageRequest {
  repairShop: string;
  actualRepairCost: number;
  repairedDate: string;
  repairNotes?: string;
}

export interface DamageInsuranceRequest {
  claimNumber: string;
  reimbursement?: number;
}

export interface DamagePhotosRequest {
  photoUrls: string[];
}

export enum DamageType {
  Collision = 0,
  Scratch = 1,
  Dent = 2,
  BrokenGlass = 3,
  MechanicalFailure = 4,
  Vandalism = 5,
  WeatherDamage = 6,
  WearAndTear = 7,
  Theft = 8,
  Other = 99
}

export enum DamageSeverity {
  Minor = 0,
  Moderate = 1,
  Major = 2,
  Critical = 3
}

export enum DamageStatus {
  Reported = 0,
  UnderAssessment = 1,
  UnderRepair = 2,
  Repaired = 3,
  WriteOff = 4
}

// ============ MILEAGE LOG ============

export interface VehicleMileageLog {
  id: string;
  vehicleId: string;
  vehicleLicensePlate: string;
  type: MileageLogType;
  typeName: string;
  startMileage: number;
  endMileage: number;
  distance: number;
  startLatitude?: number;
  startLongitude?: number;
  startAddress?: string;
  endLatitude?: number;
  endLongitude?: number;
  endAddress?: string;
  startDateTime: string;
  endDateTime?: string;
  duration?: string;
  driverId?: string;
  driverName?: string;
  shipmentId?: string;
  shipmentNumber?: string;
  fuelConsumed?: number;
  fuelCost?: number;
  fuelEfficiency?: number;
  status: MileageLogStatus;
  statusName: string;
  tollCost?: number;
  parkingCost?: number;
  otherCosts?: number;
  totalCost: number;
  purpose?: string;
  notes?: string;
  routePolyline?: string;
  createdAt: string;
}

export interface CreateMileageLogRequest {
  vehicleId?: string;
  type: MileageLogType;
  startMileage: number;
  startDateTime: string;
  startLatitude?: number;
  startLongitude?: number;
  startAddress?: string;
  driverId?: string;
  driverName?: string;
  shipmentId?: string;
  shipmentNumber?: string;
  purpose?: string;
}

export interface CompleteMileageLogRequest {
  endMileage: number;
  endDateTime: string;
  endLatitude?: number;
  endLongitude?: number;
  endAddress?: string;
  routePolyline?: string;
  fuelConsumed?: number;
  fuelCost?: number;
  tollCost?: number;
  parkingCost?: number;
  otherCosts?: number;
}

export interface UpdateMileageLogCostsRequest {
  fuelConsumed?: number;
  fuelCost?: number;
  tollCost?: number;
  parkingCost?: number;
  otherCosts?: number;
}

export enum MileageLogType {
  Delivery = 0,
  Pickup = 1,
  Transfer = 2,
  Maintenance = 3,
  Inspection = 4,
  Refueling = 5,
  Administrative = 6,
  Personal = 7,
  Service = 8,
  Business = 9,
  Commute = 10,
  Other = 99
}

export enum MileageLogStatus {
  InProgress = 0,
  Completed = 1,
  Cancelled = 2,
  Planned = 3
}

// ============ ALERTS & SUMMARY ============

export interface VehicleAlert {
  vehicleId: string;
  vehicleLicensePlate: string;
  alertType: string;
  title: string;
  description: string;
  dueDate?: string;
  daysUntilDue?: number;
  severity: 'Critical' | 'High' | 'Medium' | 'Low';
}

export interface VehicleSummary {
  vehicleId: string;
  licensePlate: string;
  model: string;
  currentMileage: number;
  totalMaintenanceCost: number;
  totalDamageCost: number;
  totalDeliveries: number;
  pendingDamages: number;
  expiringDocuments: number;
  expiringInspections: number;
  nextMaintenanceDate?: string;
  nextInspectionDate?: string;
  alerts: VehicleAlert[];
}

// ============ API RESPONSES ============

export interface VehicleMaintenanceResponse {
  success: boolean;
  data: VehicleMaintenance;
  message?: string;
}

export interface VehicleMaintenanceListResponse {
  success: boolean;
  data: VehicleMaintenance[];
  message?: string;
}

export interface VehicleInspectionResponse {
  success: boolean;
  data: VehicleInspection;
  message?: string;
}

export interface VehicleInspectionListResponse {
  success: boolean;
  data: VehicleInspection[];
  message?: string;
}

export interface VehicleDocumentResponse {
  success: boolean;
  data: VehicleDocument;
  message?: string;
}

export interface VehicleDocumentListResponse {
  success: boolean;
  data: VehicleDocument[];
  message?: string;
}

export interface VehicleDamageResponse {
  success: boolean;
  data: VehicleDamage;
  message?: string;
}

export interface VehicleDamageListResponse {
  success: boolean;
  data: VehicleDamage[];
  message?: string;
}

export interface VehicleMileageLogResponse {
  success: boolean;
  data: VehicleMileageLog;
  message?: string;
}

export interface VehicleMileageLogListResponse {
  success: boolean;
  data: VehicleMileageLog[];
  message?: string;
}

export interface VehicleAlertListResponse {
  success: boolean;
  data: VehicleAlert[];
  message?: string;
}

export interface VehicleSummaryResponse {
  success: boolean;
  data: VehicleSummary;
  message?: string;
}
