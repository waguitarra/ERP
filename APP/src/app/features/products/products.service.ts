import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { Product, CreateProductDto, UpdateProductDto, ProductListResponse } from '@core/models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/products';

  getAll(companyId?: string): Promise<ProductListResponse> {
    // Backend: GET /api/products?companyId={guid}
    const params: any = {};
    if (companyId) params.companyId = companyId;
    return this.api.get<ProductListResponse>(this.endpoint, params);
  }

  getById(id: string): Promise<Product> {
    return this.api.get<Product>(`${this.endpoint}/${id}`);
  }

  create(data: CreateProductDto): Promise<Product> {
    return this.api.post<Product>(this.endpoint, data);
  }

  update(id: string, data: UpdateProductDto): Promise<void> {
    return this.api.put<void>(`${this.endpoint}/${id}`, data);
  }

  delete(id: string): Promise<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
