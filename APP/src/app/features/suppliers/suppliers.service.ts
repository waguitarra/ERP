import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Supplier, CreateSupplierDto, UpdateSupplierDto } from '@core/models/supplier.model';

@Injectable({
  providedIn: 'root'
})
export class SuppliersService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/suppliers';

  getAll(companyId?: string): Promise<any> {
    const params: any = {};
    if (companyId) params.companyId = companyId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: string): Promise<Supplier> {
    return this.api.get<Supplier>(`${this.endpoint}/${id}`);
  }

  create(data: CreateSupplierDto): Promise<Supplier> {
    return this.api.post<Supplier>(this.endpoint, data);
  }

  update(id: string, data: UpdateSupplierDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
