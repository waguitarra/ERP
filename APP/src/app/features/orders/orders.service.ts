import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Order, CreateOrderRequest, UpdateOrderDto } from '@core/models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/orders';

  getAll(companyId?: string): Promise<any> {
    // Backend Orders não tem paginação, só GET /api/orders/company/{companyId}
    // Por ora retorna array vazio se não tiver companyId
    if (!companyId) return Promise.resolve({ success: true, data: [] });
    return this.api.get<any>(`${this.endpoint}/company/${companyId}`);
  }

  getById(id: string): Promise<any> {
    return this.api.get<any>(`${this.endpoint}/${id}`);
  }

  create(data: CreateOrderRequest): Promise<any> {
    return this.api.post<any>(this.endpoint, data);
  }

  update(id: string, data: UpdateOrderDto): Promise<any> {
    return this.api.put<any>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<any> {
    return this.api.delete<any>(`${this.endpoint}/${id}`);
  }
}
