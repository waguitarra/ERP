import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';

export interface ProductCategory {
  id: string;
  name: string;
  code: string;
  description?: string;
  barcode?: string;
  reference?: string;
  isMaintenance: boolean;
  isActive: boolean;
  attributes?: string;
  createdAt: string;
  updatedAt?: string;
  products?: any[];
}

export interface CreateProductCategoryRequest {
  name: string;
  code: string;
  description?: string;
  barcode?: string;
  reference?: string;
  isMaintenance?: boolean;
  attributes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProductCategoriesService {
  private readonly api = inject(ApiService);
  private readonly baseUrl = '/api/product-categories';

  async getAll(): Promise<ProductCategory[]> {
    return this.api.get<ProductCategory[]>(this.baseUrl);
  }

  async getActive(): Promise<ProductCategory[]> {
    return this.api.get<ProductCategory[]>(`${this.baseUrl}/active`);
  }

  async getById(id: string): Promise<ProductCategory> {
    return this.api.get<ProductCategory>(`${this.baseUrl}/${id}`);
  }

  async getByCode(code: string): Promise<ProductCategory> {
    return this.api.get<ProductCategory>(`${this.baseUrl}/by-code/${code}`);
  }

  async create(data: CreateProductCategoryRequest): Promise<ProductCategory> {
    return this.api.post<ProductCategory>(this.baseUrl, data);
  }

  async update(id: string, data: CreateProductCategoryRequest): Promise<ProductCategory> {
    return this.api.put<ProductCategory>(`${this.baseUrl}/${id}`, data);
  }

  async activate(id: string): Promise<ProductCategory> {
    return this.api.post<ProductCategory>(`${this.baseUrl}/${id}/activate`, {});
  }

  async deactivate(id: string): Promise<ProductCategory> {
    return this.api.post<ProductCategory>(`${this.baseUrl}/${id}/deactivate`, {});
  }

  async delete(id: string): Promise<void> {
    return this.api.delete(`${this.baseUrl}/${id}`);
  }
}
