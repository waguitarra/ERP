import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { I18nService } from './i18n.service';

export interface OrderPriorityResponse {
  id: number;
  code: string;
  name: string;
  description?: string;
  colorHex: string;
  sortOrder: number;
}

@Injectable({ providedIn: 'root' })
export class OrderPriorityService {
  private readonly api = inject(ApiService);
  private readonly i18n = inject(I18nService);

  async getAll(): Promise<OrderPriorityResponse[]> {
    const language = this.i18n.currentLanguage().split('-')[0]; // pt-BR -> pt
    const response: any = await this.api.get<any>(`/orderpriority?language=${language}`);
    return response.data || response || [];
  }

  async getById(id: number): Promise<OrderPriorityResponse | null> {
    const language = this.i18n.currentLanguage().split('-')[0]; // pt-BR -> pt
    try {
      const response: any = await this.api.get<any>(`/orderpriority/${id}?language=${language}`);
      return response.data || response;
    } catch {
      return null;
    }
  }

  async getByCode(code: string): Promise<OrderPriorityResponse | null> {
    const language = this.i18n.currentLanguage().split('-')[0]; // pt-BR -> pt
    try {
      const response: any = await this.api.get<any>(`/orderpriority/code/${code}?language=${language}`);
      return response.data || response;
    } catch {
      return null;
    }
  }
}
