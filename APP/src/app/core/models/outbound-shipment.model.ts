// Status enum matching backend OutboundStatus
export enum OutboundStatus {
  Pending = 1,
  ReadyToShip = 2,
  Shipped = 3,
  InTransit = 4,
  Delivered = 5,
  Cancelled = 6
}

export interface OutboundShipment {
  id: string;
  shipmentNumber: string;
  orderId: string;
  orderNumber?: string;
  carrierId?: string;
  trackingNumber?: string;
  deliveryAddress?: string;
  status: OutboundStatus;
  statusName?: string;
  shippedDate?: string;
  deliveredDate?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface OutboundShipmentListResponse {
  success: boolean;
  data: OutboundShipment[];
  message?: string;
}

export interface OutboundShipmentResponse {
  success: boolean;
  data: OutboundShipment;
  message?: string;
}

export interface CreateOutboundShipmentRequest {
  shipmentNumber: string;
  orderId: string;
  carrierId?: string;
  trackingNumber?: string;
  deliveryAddress?: string;
}
