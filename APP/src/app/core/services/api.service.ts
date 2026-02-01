import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '@environments/environment';

export interface QueryParams {
  [key: string]: string | number | boolean | string[] | number[];
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  // GET request
  get<T>(endpoint: string, params?: QueryParams): Promise<T> {
    const httpParams = this.buildHttpParams(params);
    return firstValueFrom(
      this.http.get<T>(`${this.baseUrl}${endpoint}`, { params: httpParams })
    );
  }

  // POST request
  post<T>(endpoint: string, body: any): Promise<T> {
    return firstValueFrom(
      this.http.post<T>(`${this.baseUrl}${endpoint}`, body)
    );
  }

  // PUT request
  put<T>(endpoint: string, body: any): Promise<T> {
    return firstValueFrom(
      this.http.put<T>(`${this.baseUrl}${endpoint}`, body)
    );
  }

  // DELETE request
  delete<T>(endpoint: string): Promise<T> {
    return firstValueFrom(
      this.http.delete<T>(`${this.baseUrl}${endpoint}`)
    );
  }

  // PATCH request
  patch<T>(endpoint: string, body: any): Promise<T> {
    return firstValueFrom(
      this.http.patch<T>(`${this.baseUrl}${endpoint}`, body)
    );
  }

  // Helper para construir HttpParams
  private buildHttpParams(params?: QueryParams): HttpParams {
    let httpParams = new HttpParams();
    
    if (params) {
      Object.keys(params).forEach(key => {
        const value = params[key];
        if (value !== null && value !== undefined) {
          if (Array.isArray(value)) {
            value.forEach(v => {
              httpParams = httpParams.append(key, v.toString());
            });
          } else {
            httpParams = httpParams.append(key, value.toString());
          }
        }
      });
    }
    
    return httpParams;
  }
}
