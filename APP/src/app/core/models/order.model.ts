export interface Order {
  id: number;
  orderNumber: string;
  customerId: number;
  customerName: string;
  status: OrderStatus;
  totalAmount: number;
  orderDate: Date;
  deliveryDate?: Date;
  items: OrderItem[];
  createdAt: Date;
  updatedAt: Date;
}

export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export type OrderStatus = 'Pendente' | 'Processando' | 'Enviado' | 'Entregue' | 'Cancelado';

export interface CreateOrderDto {
  customerId: number;
  orderDate: Date;
  deliveryDate?: Date;
  items: CreateOrderItemDto[];
}

export interface CreateOrderItemDto {
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface UpdateOrderDto {
  status?: OrderStatus;
  deliveryDate?: Date;
}
