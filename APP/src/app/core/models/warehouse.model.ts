export interface Warehouse {
  id: string;
  companyId: string;
  name: string;
  code: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  country?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateWarehouseDto {
  companyId: string;
  name: string;
  code: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  country?: string;
}

export interface UpdateWarehouseDto {
  name?: string;
  code?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  isActive?: boolean;
}
