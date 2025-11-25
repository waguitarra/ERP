import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { I18nService } from './i18n.service';

export interface OrderStatusResponse {
  id: number;
  code: string;
  name: string;
  description?: string;
  colorHex: string;
  sortOrder: number;
}

@Injectable({ providedIn: 'root' })
export class OrderStatusService {
  private readonly api = inject(ApiService);
  private readonly i18n = inject(I18nService);

  async getAll(): Promise<OrderStatusResponse[]> {
    const language = this.i18n.currentLanguage().split('-')[0]; // pt-BR -> pt
    const response: any = await this.api.get<any>(`/orderstatus?language=${language}`);
    return response.data || response || [];
  }

  async getById(id: number): Promise<OrderStatusResponse | null> {
    const language = this.i18n.currentLanguage().split('-')[0]; // pt-BR -> pt
    try {
      const response: any = await this.api.get<any>(`/orderstatus/${id}?language=${language}`);
      return response.data || response;
    } catch {
      return null;
    }
  }

  async getByCode(code: string): Promise<OrderStatusResponse | null> {
    const language = this.i18n.currentLanguage().split('-')[0]; // pt-BR -> pt
    try {
      const response: any = await this.api.get<any>(`/orderstatus/code/${code}?language=${language}`);
      return response.data || response;
    } catch {
      return null;
    }
  }
}
