export interface Supplier {
  id: number;
  name: string;
  email: string;
  phone: string;
  document: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface CreateSupplierDto {
  name: string;
  email: string;
  phone: string;
  document: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
}

export interface UpdateSupplierDto {
  name?: string;
  email?: string;
  phone?: string;
  document?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  isActive?: boolean;
}
