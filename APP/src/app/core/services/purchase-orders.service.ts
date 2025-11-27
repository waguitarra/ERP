import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';

export interface PurchaseOrderItem {
  productId: string;
  sku: string;
  quantityOrdered: number;
  quantityReceived?: number;
  unitPrice: number;
}

export interface PurchaseOrder {
  id: string;
  companyId: string;
  purchaseOrderNumber: string;
  supplierId: string;
  status: string;
  expectedDate?: string;
  priority: number;
  totalQuantity: number;
  totalValue: number;
  unitCost?: number;
  taxPercentage?: number;
  desiredMarginPercentage?: number;
  suggestedSalePrice?: number;
  expectedParcels?: number;
  receivedParcels?: number;
  cartonsPerParcel?: number;
  unitsPerCarton?: number;
  destinationWarehouseId?: string;
  vehicleId?: string;
  driverId?: string;
  dockDoorNumber?: string;
  shippingDistance?: string;
  shippingCost?: number;
  isInternational?: boolean;
  originCountry?: string;
  containerNumber?: string;
  incoterm?: string;
  portOfEntry?: string;
  createdAt: string;
  updatedAt?: string;
  items: PurchaseOrderItem[];
}

export interface CreatePurchaseOrderRequest {
  companyId: string;
  purchaseOrderNumber: string;
  supplierId: string;
  expectedDate?: string;
  priority?: number;
  items: PurchaseOrderItem[];
}

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrdersService {
  private readonly api = inject(ApiService);
  private readonly baseUrl = '/api/purchase-orders';

  async getAll(companyId: string): Promise<PurchaseOrder[]> {
    return this.api.get<PurchaseOrder[]>(`${this.baseUrl}/company/${companyId}`);
  }

  async getById(id: string): Promise<PurchaseOrder> {
    return this.api.get<PurchaseOrder>(`${this.baseUrl}/${id}`);
  }

  async getByNumber(orderNumber: string, companyId: string): Promise<PurchaseOrder> {
    return this.api.get<PurchaseOrder>(`${this.baseUrl}/by-number/${orderNumber}/${companyId}`);
  }

  async create(data: CreatePurchaseOrderRequest): Promise<PurchaseOrder> {
    return this.api.post<PurchaseOrder>(this.baseUrl, data);
  }

  async update(id: string, data: Partial<CreatePurchaseOrderRequest>): Promise<PurchaseOrder> {
    return this.api.put<PurchaseOrder>(`${this.baseUrl}/${id}`, data);
  }

  async setPurchaseDetails(id: string, data: {
    unitCost: number;
    taxPercentage: number;
    desiredMarginPercentage: number;
  }): Promise<PurchaseOrder> {
    return this.api.post<PurchaseOrder>(`${this.baseUrl}/${id}/purchase-details`, data);
  }

  async setPackagingHierarchy(id: string, data: {
    expectedParcels: number;
    cartonsPerParcel: number;
    unitsPerCarton: number;
  }): Promise<PurchaseOrder> {
    return this.api.post<PurchaseOrder>(`${this.baseUrl}/${id}/packaging-hierarchy`, data);
  }

  async setInternational(id: string, data: {
    originCountry: string;
    portOfEntry: string;
    containerNumber: string;
    incoterm: string;
  }): Promise<PurchaseOrder> {
    return this.api.post<PurchaseOrder>(`${this.baseUrl}/${id}/set-international`, data);
  }

  async setLogistics(id: string, data: {
    destinationWarehouseId?: string;
    vehicleId?: string;
    driverId?: string;
    dockDoorNumber?: string;
    shippingDistance?: string;
    shippingCost?: number;
  }): Promise<PurchaseOrder> {
    return this.api.post<PurchaseOrder>(`${this.baseUrl}/${id}/set-logistics`, data);
  }

  async delete(id: string): Promise<void> {
    return this.api.delete(`${this.baseUrl}/${id}`);
  }
}
