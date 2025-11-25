export interface Inventory {
  id: string;
  warehouseId: string;
  productId: string;
  locationId?: string;
  quantity: number;
  availableQuantity: number;
  reservedQuantity: number;
  minimumStock?: number;
  maximumStock?: number;
  lotNumber?: string;
  serialNumber?: string;
  expirationDate?: Date;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateInventoryDto {
  warehouseId: string;
  productId: string;
  locationId?: string;
  quantity: number;
  lotNumber?: string;
  serialNumber?: string;
  expirationDate?: Date;
}

export interface UpdateInventoryDto {
  quantity?: number;
  locationId?: string;
  minimumStock?: number;
  maximumStock?: number;
}

export interface InventoryAdjustmentDto {
  warehouseId: string;
  productId: string;
  locationId?: string;
  quantityChange: number;
  reason: string;
  notes?: string;
}

export interface InventoryReservationDto {
  warehouseId: string;
  productId: string;
  quantity: number;
  orderId?: string;
  notes?: string;
}
