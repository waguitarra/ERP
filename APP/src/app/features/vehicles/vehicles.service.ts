import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Vehicle, CreateVehicleDto, UpdateVehicleDto } from '@core/models/vehicle.model';

@Injectable({
  providedIn: 'root'
})
export class VehiclesService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/vehicles';

  getAll(companyId?: string): Promise<any> {
    // Backend: GET /api/vehicles?companyId={guid}
    const params: any = {};
    if (companyId) params.companyId = companyId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: string): Promise<Vehicle> {
    return this.api.get<Vehicle>(`${this.endpoint}/${id}`);
  }

  create(data: CreateVehicleDto): Promise<Vehicle> {
    return this.api.post<Vehicle>(this.endpoint, data);
  }

  update(id: string, data: UpdateVehicleDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
