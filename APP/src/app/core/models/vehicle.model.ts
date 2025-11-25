import { VehicleStatus } from './enums';

export interface Vehicle {
  id: string;  // Guid
  companyId: string;  // Guid
  plateNumber: string;  // ⚠️ Backend usa "PlateNumber", não "licensePlate"
  model: string;
  capacity?: number;
  vehicleType?: string;
  status: VehicleStatus;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateVehicleDto {
  companyId: string;
  plateNumber: string;
  model: string;
  capacity?: number;
  vehicleType?: string;
  status?: VehicleStatus;
}

export interface UpdateVehicleDto {
  plateNumber?: string;
  model?: string;
  capacity?: number;
  vehicleType?: string;
  status?: VehicleStatus;
  isActive?: boolean;
}
