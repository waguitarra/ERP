import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Company, CreateCompanyDto, UpdateCompanyDto } from '@core/models/company.model';

@Injectable({
  providedIn: 'root'
})
export class CompaniesService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/companies';

  getAll(): Promise<any> {
    return this.api.get<any>(this.endpoint);
  }

  getById(id: string): Promise<Company> {
    return this.api.get<Company>(`${this.endpoint}/${id}`);
  }

  create(data: CreateCompanyDto): Promise<Company> {
    return this.api.post<Company>(this.endpoint, data);
  }

  update(id: string, data: UpdateCompanyDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
