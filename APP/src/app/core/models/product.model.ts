export interface Product {
  id: string;  // Guid do backend
  companyId: string;  // Guid - OBRIGATÓRIO para multi-tenancy
  name: string;
  sku: string;
  barcode?: string;
  description?: string;
  
  // Dimensões e Peso
  weight: number;
  weightUnit?: string;  // kg, g, lb
  volume: number;
  volumeUnit?: string;  // m3, cm3
  length: number;
  width: number;
  height: number;
  dimensionUnit?: string;  // cm, m, in
  
  // WMS Tracking
  requiresLotTracking: boolean;
  requiresSerialTracking: boolean;
  isPerishable: boolean;
  shelfLifeDays?: number;
  
  // Estoque
  minimumStock?: number;
  safetyStock?: number;
  abcClassification?: string;  // A, B, C
  
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateProductDto {
  companyId: string;  // SEMPRE obrigatório
  categoryId: string;
  name: string;
  sku: string;
  barcode?: string;
  description?: string;
  weight: number;
  weightUnit?: string;
  volume?: number;
  volumeUnit?: string;
  length?: number;
  width?: number;
  height?: number;
  dimensionUnit?: string;
  requiresLotTracking?: boolean;
  requiresSerialTracking?: boolean;
  isPerishable?: boolean;
  shelfLifeDays?: number;
  minimumStock?: number;
  safetyStock?: number;
  abcClassification?: string;
}

export interface UpdateProductDto {
  name?: string;
  sku?: string;
  barcode?: string;
  description?: string;
  weight?: number;
  weightUnit?: string;
  volume?: number;
  volumeUnit?: string;
  length?: number;
  width?: number;
  height?: number;
  dimensionUnit?: string;
  requiresLotTracking?: boolean;
  requiresSerialTracking?: boolean;
  isPerishable?: boolean;
  shelfLifeDays?: number;
  minimumStock?: number;
  safetyStock?: number;
  abcClassification?: string;
  isActive?: boolean;
}

export interface ProductListResponse {
  data: Product[];
  total: number;
  page: number;
  pageSize: number;
}
