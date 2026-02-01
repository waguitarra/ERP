import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly TOKEN_KEY = 'WMS_auth_token';
  private readonly USER_KEY = 'WMS_user_data';

  // Token methods
  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  clearToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  // User data methods
  setUser(userData: any): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(userData));
  }

  getUser<T>(): T | null {
    const userData = localStorage.getItem(this.USER_KEY);
    if (!userData || userData === 'undefined' || userData === 'null') {
      return null;
    }
    try {
      return JSON.parse(userData);
    } catch (error) {
      console.error('Erro ao parsear userData:', error);
      localStorage.removeItem(this.USER_KEY);
      return null;
    }
  }

  clearUser(): void {
    localStorage.removeItem(this.USER_KEY);
  }

  // Clear all
  clearAll(): void {
    this.clearToken();
    this.clearUser();
  }
}
