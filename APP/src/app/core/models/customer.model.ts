export interface Customer {
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

export interface CreateCustomerDto {
  name: string;
  email: string;
  phone: string;
  document: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
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
  isActive?: boolean;
}
