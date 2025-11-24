import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';

export interface Driver {
  id: string;
  companyId: string;
  name: string;
  licenseNumber: string;
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class DriversService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/drivers';

  getAll(companyId?: string): Promise<any> {
    // Backend: GET /api/drivers?companyId={guid}
    const params: any = {};
    if (companyId) params.companyId = companyId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: string): Promise<Driver> {
    return this.api.get<Driver>(`${this.endpoint}/${id}`);
  }

  create(data: any): Promise<Driver> {
    return this.api.post<Driver>(this.endpoint, data);
  }

  update(id: string, data: any): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
