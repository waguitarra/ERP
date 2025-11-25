import { OrderType, OrderStatus, OrderSource, OrderPriority } from './enums';

export interface Order {
  id: string;  // Guid
  companyId: string;  // Guid
  orderNumber: string;
  type: OrderType;
  source: OrderSource;
  customerId?: string;  // Guid nullable
  customerName?: string;
  supplierId?: string;  // Guid nullable
  supplierName?: string;
  orderDate: Date;
  expectedDate?: Date;
  priority: OrderPriority;
  status: OrderStatus;
  totalQuantity: number;
  totalValue: number;
  shippingAddress?: string;
  specialInstructions?: string;
  isBOPIS: boolean;  // Buy Online Pickup In Store
  createdBy: string;  // Guid
  items: OrderItem[];
  createdAt: Date;
  updatedAt?: Date;
}

export interface OrderItem {
  id: string;  // Guid
  orderId: string;  // Guid
  productId: string;  // Guid
  productName?: string;
  sku: string;
  quantityOrdered: number;
  quantityAllocated: number;
  quantityPicked: number;
  quantityShipped: number;
  unitPrice: number;
  requiredLotNumber?: string;
  requiredShipDate?: Date;
}

export interface CreateOrderRequest {
  companyId: string;
  orderNumber: string;
  type: OrderType;
  source: OrderSource;
  customerId?: string;
  supplierId?: string;
  orderDate: Date | string;
  expectedDate?: Date | string;
  priority: OrderPriority;
  shippingAddress?: string;
  specialInstructions?: string;
  isBOPIS?: boolean;
  items: CreateOrderItemRequest[];
}

export interface CreateOrderItemRequest {
  productId: string;
  sku: string;
  quantityOrdered: number;
  unitPrice: number;
  requiredLotNumber?: string;
  requiredShipDate?: Date;
}

export interface UpdateOrderDto {
  status?: OrderStatus;
  priority?: OrderPriority;
  expectedDate?: Date | string;
  shippingAddress?: string;
  specialInstructions?: string;
}
