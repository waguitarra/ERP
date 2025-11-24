import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';

export interface Inventory {
  id: string;
  warehouseId: string;
  productId: string;
  quantity: number;
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/inventories';

  getAll(warehouseId?: string, productId?: string): Promise<any> {
    // Backend: GET /api/inventories?warehouseId={guid}&productId={guid}
    const params: any = {};
    if (warehouseId) params.warehouseId = warehouseId;
    if (productId) params.productId = productId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: string): Promise<Inventory> {
    return this.api.get<Inventory>(`${this.endpoint}/${id}`);
  }

  create(data: any): Promise<Inventory> {
    return this.api.post<Inventory>(this.endpoint, data);
  }

  update(id: string, data: any): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
