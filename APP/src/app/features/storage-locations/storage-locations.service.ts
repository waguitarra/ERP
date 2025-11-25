import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { StorageLocation, CreateStorageLocationDto, UpdateStorageLocationDto, BlockLocationDto } from '@core/models/storage-location.model';

@Injectable({
  providedIn: 'root'
})
export class StorageLocationsService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/storagelocations';

  getAll(warehouseId?: string): Promise<any> {
    const params: any = {};
    if (warehouseId) params.warehouseId = warehouseId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: string): Promise<StorageLocation> {
    return this.api.get<StorageLocation>(`${this.endpoint}/${id}`);
  }

  create(data: CreateStorageLocationDto): Promise<StorageLocation> {
    return this.api.post<StorageLocation>(this.endpoint, data);
  }

  update(id: string, data: UpdateStorageLocationDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  block(id: string, data: BlockLocationDto): Promise<void> {
    return this.api.post<void>(`${this.endpoint}/${id}/block`, data);
  }

  unblock(id: string): Promise<void> {
    return this.api.post<void>(`${this.endpoint}/${id}/unblock`, {});
  }
}
