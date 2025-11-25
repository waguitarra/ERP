export interface Supplier {
  id: string;  // Guid
  companyId: string;  // Guid
  name: string;
  email: string;
  phone: string;
  document: string;  // CPF/CNPJ
  address: string;
  city: string;
  state: string;
  zipCode: string;
  country?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateSupplierDto {
  companyId: string;
  name: string;
  email: string;
  phone: string;
  document: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  country?: string;
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
  country?: string;
  isActive?: boolean;
}
