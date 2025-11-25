export interface Customer {
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
  latitude?: number;
  longitude?: number;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateCustomerDto {
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

export interface UpdateCustomerDto {
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
