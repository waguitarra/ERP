import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { StorageService } from './storage.service';
import { ApiService } from './api.service';

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface LoginData {
  token: string;
  email: string;
  name: string;
  role: string;
  companyId: string | null;
}

export interface LoginResponse {
  success: boolean;
  data: LoginData;
  message: string;
  errors: string[];
}

export interface User {
  email: string;
  name: string;
  role: string;
  companyId: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiService = inject(ApiService);
  private readonly storageService = inject(StorageService);
  private readonly router = inject(Router);

  currentUser = signal<User | null>(null);
  isLoading = signal<boolean>(false);

  constructor() {
    // Carrega usuário do storage na inicialização
    const user = this.storageService.getUser<User>();
    if (user) {
      this.currentUser.set(user);
    }
  }

  async login(credentials: LoginCredentials): Promise<void> {
    this.isLoading.set(true);
    try {
      const response = await this.apiService.post<LoginResponse>('/auth/login', credentials);
      
      if (!response.success || !response.data) {
        throw new Error(response.message || 'Erro ao fazer login');
      }

      const user: User = {
        email: response.data.email,
        name: response.data.name,
        role: response.data.role,
        companyId: response.data.companyId
      };
      
      this.storageService.setToken(response.data.token);
      this.storageService.setUser(user);
      this.currentUser.set(user);
      
      this.router.navigate(['/dashboard']);
    } catch (error) {
      throw error;
    } finally {
      this.isLoading.set(false);
    }
  }

  logout(): void {
    this.storageService.clearAll();
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    return !!this.storageService.getToken();
  }

  isAdmin(): boolean {
    const user = this.currentUser();
    return user?.role === 'Admin';
  }
}
