export interface Driver {
  id: string;
  companyId: string;
  name: string;
  licenseNumber: string;
  phone: string;
  email?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateDriverDto {
  companyId: string;
  name: string;
  licenseNumber: string;
  phone: string;
  email?: string;
}

export interface UpdateDriverDto {
  name?: string;
  licenseNumber?: string;
  phone?: string;
  email?: string;
  isActive?: boolean;
}
