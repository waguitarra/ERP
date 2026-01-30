import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { PickingTask, PickingTaskListResponse, PickingTaskResponse } from '@core/models/picking-task.model';

@Injectable({
  providedIn: 'root'
})
export class PickingTasksService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/picking-tasks';

  getAll(): Promise<PickingTaskListResponse> {
    return this.api.get<PickingTaskListResponse>(this.endpoint);
  }

  getById(id: string): Promise<PickingTaskResponse> {
    return this.api.get<PickingTaskResponse>(`${this.endpoint}/${id}`);
  }

  getPending(): Promise<PickingTaskListResponse> {
    return this.api.get<PickingTaskListResponse>(`${this.endpoint}/pending`);
  }

  getInProgress(): Promise<PickingTaskListResponse> {
    return this.api.get<PickingTaskListResponse>(`${this.endpoint}/in-progress`);
  }

  getByStatus(status: number): Promise<PickingTaskListResponse> {
    return this.api.get<PickingTaskListResponse>(`${this.endpoint}/status/${status}`);
  }

  getByWave(waveId: string): Promise<PickingTaskListResponse> {
    return this.api.get<PickingTaskListResponse>(`${this.endpoint}/wave/${waveId}`);
  }

  assign(taskId: string, userId: string): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${taskId}/assign`, { userId });
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

  pickLine(taskId: string, lineId: string, quantityPicked: number): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${taskId}/lines/${lineId}/pick`, { quantityPicked });
  }
}
