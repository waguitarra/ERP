import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { 
  PackingTask, 
  PackingTaskListResponse, 
  PackingTaskResponse,
  CreatePackingTaskRequest 
} from '@core/models/packing-task.model';

@Injectable({
  providedIn: 'root'
})
export class PackingTasksService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/packing-tasks';

  getAll(): Promise<PackingTaskListResponse> {
    return this.api.get<PackingTaskListResponse>(this.endpoint);
  }

  getById(id: string): Promise<PackingTaskResponse> {
    return this.api.get<PackingTaskResponse>(`${this.endpoint}/${id}`);
  }

  getPending(): Promise<PackingTaskListResponse> {
    return this.api.get<PackingTaskListResponse>(`${this.endpoint}/pending`);
  }

  getInProgress(): Promise<PackingTaskListResponse> {
    return this.api.get<PackingTaskListResponse>(`${this.endpoint}/in-progress`);
  }

  getByStatus(status: number): Promise<PackingTaskListResponse> {
    return this.api.get<PackingTaskListResponse>(`${this.endpoint}/status/${status}`);
  }

  getByOrder(orderId: string): Promise<PackingTaskListResponse> {
    return this.api.get<PackingTaskListResponse>(`${this.endpoint}/order/${orderId}`);
  }

  create(request: CreatePackingTaskRequest): Promise<PackingTaskResponse> {
    return this.api.post<PackingTaskResponse>(this.endpoint, request);
  }

  start(taskId: string): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${taskId}/start`, {});
  }

  complete(taskId: string): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${taskId}/complete`, {});
  }

  cancel(taskId: string): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${taskId}/cancel`, {});
  }

  delete(taskId: string): Promise<any> {
    return this.api.delete<any>(`${this.endpoint}/${taskId}`);
  }
}
