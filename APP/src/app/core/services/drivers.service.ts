import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';

export interface Driver {
  id: string;
  name: string;
  licenseNumber: string;
  phone: string;
  email?: string;
  companyId: string;
  isActive: boolean;
}

@Injectable({ providedIn: 'root' })
export class DriversService {
  private readonly api = inject(ApiService);

  async getAll(companyId?: string): Promise<Driver[]> {
    const url = companyId ? `/drivers?companyId=${companyId}` : '/drivers';
    const response: any = await this.api.get<any>(url);
    return response.data || response || [];
  }

  async getById(id: string): Promise<Driver | null> {
    try {
      const response: any = await this.api.get<any>(`/drivers/${id}`);
      return response.data || response;
    } catch {
      return null;
    }
  }
}
