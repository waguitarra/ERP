export enum VehicleStatus {
  Available = 0,
  InTransit = 1,
  Maintenance = 2,
  Inactive = 3
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

export enum InspectionType {
  ITV = 0,         // Espanha
  MOT = 1,         // UK
  TUV = 2,         // Alemanha
  CT = 3,          // França
  Revisione = 4,   // Itália
  APK = 5,         // Holanda
  DETRAN = 6,      // Brasil
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

export interface Vehicle {
  id: string;
  companyId: string;
  companyName?: string;
  licensePlate: string;
  model: string;
  brand?: string;
  vehicleType?: string;
  year: number;
  capacity?: number;
  color?: string;
  fuelType?: string;
  notes?: string;
  status: number;
  statusName: string;
  
  // Tracking fields
  trackingToken?: string;
  trackingEnabled: boolean;
  lastLatitude?: number;
  lastLongitude?: number;
  lastLocationUpdate?: string;
  currentSpeed?: number;
  currentAddress?: string;
  isMoving: boolean;
  
  // Mileage info
  currentMileage: number;
  totalDistanceTraveled: number;
  
  // Financial info
  purchasePrice?: number;
  purchaseDate?: string;
  currentValue?: number;
  chassisNumber?: string;
  engineNumber?: string;
  
  // Insurance & Documentation
  insuranceExpiryDate?: string;
  licenseExpiryDate?: string;
  lastInspectionDate?: string;
  nextInspectionDate?: string;
  isInsuranceExpired: boolean;
  isInsuranceExpiringSoon: boolean;
  isInspectionExpired: boolean;
  isInspectionExpiringSoon: boolean;
  
  // Maintenance info
  lastMaintenanceDate?: string;
  lastMaintenanceMileage?: number;
  totalMaintenanceCost: number;
  maintenanceCount: number;
  documentCount: number;
  inspectionCount: number;
  deliveryCount: number;
  
  // Driver fields
  driverId?: string;
  driverName?: string;
  driverPhone?: string;
  driverLicenseNumber?: string;
  
  // Current shipment fields
  currentShipmentId?: string;
  currentShipmentNumber?: string;
  currentShipmentAddress?: string;
  currentShipmentStatus?: string;
  currentOrderCustomerName?: string;
  currentOrderCustomerPhone?: string;
  
  createdAt: string;
  updatedAt?: string;
}

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

export interface VehicleDeliveryHistory {
  shipmentId: string;
  shipmentNumber: string;
  shipmentDate: string;
  customerName?: string;
  deliveryAddress?: string;
  status: string;
  distance?: number;
}

export interface VehicleListResponse {
  success: boolean;
  data: Vehicle[];
  message?: string;
}

export interface VehicleResponse {
  success: boolean;
  data: Vehicle;
  message?: string;
}

export interface CreateVehicleDto {
  companyId: string;
  licensePlate: string;
  model: string;
  brand?: string;
  vehicleType?: string;
  year: number;
  capacity?: number;
  color?: string;
  fuelType?: string;
  notes?: string;
  trackingEnabled?: boolean;
}

export interface UpdateVehicleDto {
  companyId: string;
  licensePlate: string;
  model: string;
  brand?: string;
  vehicleType?: string;
  year: number;
  capacity?: number;
  color?: string;
  fuelType?: string;
  notes?: string;
  trackingEnabled?: boolean;
}

export interface UpdateVehicleLocationDto {
  latitude: number;
  longitude: number;
  speed?: number;
  address?: string;
}

export interface AssignDriverDto {
  driverName: string;
  driverPhone?: string;
}
