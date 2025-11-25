export interface StorageLocation {
  id: string;
  warehouseId: string;
  zoneId?: string;
  code: string;
  aisle?: string;
  rack?: string;
  shelf?: string;
  bin?: string;
  locationType: string;
  capacity?: number;
  currentVolume?: number;
  isBlocked: boolean;
  blockReason?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateStorageLocationDto {
  warehouseId: string;
  zoneId?: string;
  code: string;
  aisle?: string;
  rack?: string;
  shelf?: string;
  bin?: string;
  locationType: string;
  capacity?: number;
}

export interface UpdateStorageLocationDto {
  code?: string;
  aisle?: string;
  rack?: string;
  shelf?: string;
  bin?: string;
  locationType?: string;
  capacity?: number;
  isActive?: boolean;
}

export interface BlockLocationDto {
  reason: string;
}
