import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Driver, CreateDriverDto, UpdateDriverDto } from '@core/models/driver.model';

@Injectable({
  providedIn: 'root'
})
export class DriversService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/drivers';

  getAll(companyId?: string): Promise<any> {
    const params: any = {};
    if (companyId) params.companyId = companyId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: string): Promise<Driver> {
    return this.api.get<Driver>(`${this.endpoint}/${id}`);
  }

  create(data: CreateDriverDto): Promise<Driver> {
    return this.api.post<Driver>(this.endpoint, data);
  }

  update(id: string, data: UpdateDriverDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
