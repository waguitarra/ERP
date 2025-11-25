import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Warehouse, CreateWarehouseDto, UpdateWarehouseDto } from '@core/models/warehouse.model';

@Injectable({
  providedIn: 'root'
})
export class WarehousesService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/warehouses';

  getAll(companyId?: string): Promise<any> {
    const params: any = {};
    if (companyId) params.companyId = companyId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: string): Promise<Warehouse> {
    return this.api.get<Warehouse>(`${this.endpoint}/${id}`);
  }

  create(data: CreateWarehouseDto): Promise<Warehouse> {
    return this.api.post<Warehouse>(this.endpoint, data);
  }

  update(id: string, data: UpdateWarehouseDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
