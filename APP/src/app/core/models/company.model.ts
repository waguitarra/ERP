export interface Company {
  id: string;
  name: string;
  tradeName?: string;
  document: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  country?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateCompanyDto {
  name: string;
  tradeName?: string;
  document: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  country?: string;
}

export interface UpdateCompanyDto {
  name?: string;
  tradeName?: string;
  document?: string;
  email?: string;
  phone?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  isActive?: boolean;
}
