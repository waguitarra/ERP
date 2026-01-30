export enum VehicleStatus {
  Available = 0,
  InTransit = 1,
  Maintenance = 2,
  Inactive = 3
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
