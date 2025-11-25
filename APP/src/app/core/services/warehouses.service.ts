import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';

export interface Warehouse {
  id: string;
  companyId: string;
  name: string;
  code: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  latitude?: number;
  longitude?: number;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

@Injectable({ providedIn: 'root' })
export class WarehousesService {
  private readonly api = inject(ApiService);

  async getAll(companyId?: string): Promise<Warehouse[]> {
    const url = companyId ? `/warehouses?companyId=${companyId}` : '/warehouses';
    const response: any = await this.api.get<any>(url);
    // API retorna ApiResponse wrapper: {success, data, error}
    console.log('🔵 WAREHOUSES RESPONSE:', response);
    return response.data || response || [];
  }

  async getById(id: string): Promise<Warehouse | null> {
    try {
      const response: any = await this.api.get<any>(`/warehouses/${id}`);
      return response.data || response;
    } catch {
      return null;
    }
  }
}
