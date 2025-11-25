import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';

export interface Vehicle {
  id: string;
  licensePlate: string;
  model: string;
  year?: number;
  color?: string;
  capacity?: number;
  companyId: string;
  isActive: boolean;
}

@Injectable({ providedIn: 'root' })
export class VehiclesService {
  private readonly api = inject(ApiService);

  async getAll(companyId?: string): Promise<Vehicle[]> {
    const url = companyId ? `/vehicles?companyId=${companyId}` : '/vehicles';
    const response: any = await this.api.get<any>(url);
    return response.data || response || [];
  }

  async getById(id: string): Promise<Vehicle | null> {
    try {
      const response: any = await this.api.get<any>(`/vehicles/${id}`);
      return response.data || response;
    } catch {
      return null;
    }
  }
}
