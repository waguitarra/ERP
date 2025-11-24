import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Order, CreateOrderDto, UpdateOrderDto } from '@core/models/order.model';

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

  getById(id: number): Promise<Order> {
    return this.api.get<Order>(`${this.endpoint}/${id}`);
  }

  create(data: CreateOrderDto): Promise<Order> {
    return this.api.post<Order>(this.endpoint, data);
  }

  update(id: number, data: UpdateOrderDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: number): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
