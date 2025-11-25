import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Inventory, CreateInventoryDto, UpdateInventoryDto, InventoryAdjustmentDto, InventoryReservationDto } from '@core/models/inventory.model';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/inventories';

  getAll(warehouseId?: string, productId?: string): Promise<any> {
    const params: any = {};
    if (warehouseId) params.warehouseId = warehouseId;
    if (productId) params.productId = productId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: string): Promise<Inventory> {
    return this.api.get<Inventory>(`${this.endpoint}/${id}`);
  }

  create(data: CreateInventoryDto): Promise<Inventory> {
    return this.api.post<Inventory>(this.endpoint, data);
  }

  update(id: string, data: UpdateInventoryDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  adjust(data: InventoryAdjustmentDto): Promise<void> {
    return this.api.post<void>(`${this.endpoint}/adjust`, data);
  }

  reserve(data: InventoryReservationDto): Promise<void> {
    return this.api.post<void>(`${this.endpoint}/reserve`, data);
  }

  releaseReservation(inventoryId: string, quantity: number): Promise<void> {
    return this.api.post<void>(`${this.endpoint}/${inventoryId}/release`, { quantity });
  }
}
