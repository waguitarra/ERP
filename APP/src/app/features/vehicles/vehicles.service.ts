import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { 
  Vehicle, 
  CreateVehicleDto, 
  UpdateVehicleDto, 
  UpdateVehicleLocationDto,
  AssignDriverDto,
  VehicleListResponse,
  VehicleResponse
} from '@core/models/vehicle.model';

@Injectable({
  providedIn: 'root'
})
export class VehiclesService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/vehicles';

  getAll(companyId?: string): Promise<VehicleListResponse> {
    const params: any = {};
    if (companyId) params.companyId = companyId;
    return this.api.get<VehicleListResponse>(this.endpoint, params);
  }

  getById(id: string): Promise<VehicleResponse> {
    return this.api.get<VehicleResponse>(`${this.endpoint}/${id}`);
  }

  create(data: CreateVehicleDto): Promise<VehicleResponse> {
    return this.api.post<VehicleResponse>(this.endpoint, data);
  }

  update(id: string, data: UpdateVehicleDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  // Tracking methods
  enableTracking(id: string): Promise<VehicleResponse> {
    return this.api.post<VehicleResponse>(`${this.endpoint}/${id}/tracking/enable`, {});
  }

  disableTracking(id: string): Promise<VehicleResponse> {
    return this.api.post<VehicleResponse>(`${this.endpoint}/${id}/tracking/disable`, {});
  }

  regenerateTrackingToken(id: string): Promise<VehicleResponse> {
    return this.api.post<VehicleResponse>(`${this.endpoint}/${id}/tracking/regenerate-token`, {});
  }

  updateLocation(id: string, data: UpdateVehicleLocationDto): Promise<VehicleResponse> {
    return this.api.post<VehicleResponse>(`${this.endpoint}/${id}/location`, data);
  }

  getWithTrackingEnabled(): Promise<VehicleListResponse> {
    return this.api.get<VehicleListResponse>(`${this.endpoint}/tracking/enabled`);
  }

  // Driver methods
  assignDriver(id: string, data: AssignDriverDto): Promise<VehicleResponse> {
    return this.api.post<VehicleResponse>(`${this.endpoint}/${id}/driver`, data);
  }

  removeDriver(id: string): Promise<VehicleResponse> {
    return this.api.delete<VehicleResponse>(`${this.endpoint}/${id}/driver`);
  }

  // Change status
  changeStatus(id: string, status: number): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}/status`, { status });
  }
}
