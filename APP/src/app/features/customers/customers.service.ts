import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Customer, CreateCustomerDto, UpdateCustomerDto } from '@core/models/customer.model';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/customers';

  async getAll(companyId?: string): Promise<Customer[]> {
    // Backend: GET /api/customers?companyId={guid}
    const params: any = {};
    if (companyId) params.companyId = companyId;
    const response: any = await this.api.get<any>(this.endpoint, params);
    // API retorna ApiResponse wrapper: {success, data, error}
    return response.data || response || [];
  }

  getById(id: string): Promise<Customer> {
    return this.api.get<Customer>(`${this.endpoint}/${id}`);
  }

  create(data: CreateCustomerDto): Promise<Customer> {
    return this.api.post<Customer>(this.endpoint, data);
  }

  update(id: string, data: UpdateCustomerDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
