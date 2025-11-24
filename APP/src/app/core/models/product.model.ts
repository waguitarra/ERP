export interface Product {
  id: number;
  sku: string;
  name: string;
  description: string;
  category: string;
  unitPrice: number;
  weight: number;
  dimensions: string;
  barcode?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface CreateProductDto {
  sku: string;
  name: string;
  description: string;
  category: string;
  unitPrice: number;
  weight: number;
  dimensions: string;
  barcode?: string;
}

export interface UpdateProductDto {
  sku?: string;
  name?: string;
  description?: string;
  category?: string;
  unitPrice?: number;
  weight?: number;
  dimensions?: string;
  barcode?: string;
  isActive?: boolean;
}

export interface ProductListResponse {
  data: Product[];
  total: number;
  page: number;
  pageSize: number;
}
