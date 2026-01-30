export interface InboundShipment {
  id: string;
  companyId: string;
  companyName: string;
  shipmentNumber: string;
  orderId: string;
  orderNumber: string;
  supplierId: string;
  supplierName: string;
  vehicleId?: string;
  vehiclePlate?: string;
  driverId?: string;
  driverName?: string;
  expectedArrivalDate?: string;
  actualArrivalDate?: string;
  dockDoorNumber?: string;
  status: number;
  statusName: string;
  totalQuantityExpected: number;
  totalQuantityReceived: number;
  asnNumber?: string;
  hasQualityIssues: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface InboundShipmentListResponse {
  success: boolean;
  data: InboundShipment[];
  message?: string;
}

export interface InboundShipmentResponse {
  success: boolean;
  data: InboundShipment;
  message?: string;
}

export interface CreateInboundShipmentRequest {
  companyId: string;
  shipmentNumber: string;
  orderId: string;
  supplierId: string;
  vehicleId?: string;
  driverId?: string;
  expectedArrivalDate?: string;
  dockDoorNumber?: string;
  asnNumber?: string;
}

// Status enum matching backend InboundStatus
export enum InboundShipmentStatus {
  Scheduled = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}
