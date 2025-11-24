import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Customer, CreateCustomerDto, UpdateCustomerDto } from '@core/models/customer.model';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/customers';

  getAll(companyId?: string): Promise<any> {
    // Backend: GET /api/customers?companyId={guid}
    const params: any = {};
    if (companyId) params.companyId = companyId;
    return this.api.get<any>(this.endpoint, params);
  }

  getById(id: number): Promise<Customer> {
    return this.api.get<Customer>(`${this.endpoint}/${id}`);
  }

  create(data: CreateCustomerDto): Promise<Customer> {
    return this.api.post<Customer>(this.endpoint, data);
  }

  update(id: number, data: UpdateCustomerDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: number): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
