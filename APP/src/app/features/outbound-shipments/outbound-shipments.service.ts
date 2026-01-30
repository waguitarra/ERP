import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '@environments/environment';
import { OutboundShipment, CreateOutboundShipmentRequest, OutboundStatus } from '../../core/models/outbound-shipment.model';

interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class OutboundShipmentsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/outbound-shipments`;

  getAll(): Observable<OutboundShipment[]> {
    return this.http.get<ApiResponse<OutboundShipment[]>>(this.apiUrl)
      .pipe(map(response => response.data || []));
  }

  getById(id: string): Observable<OutboundShipment> {
    return this.http.get<ApiResponse<OutboundShipment>>(`${this.apiUrl}/${id}`)
      .pipe(map(response => response.data));
  }

  getByOrder(orderId: string): Observable<OutboundShipment[]> {
    return this.http.get<ApiResponse<OutboundShipment[]>>(`${this.apiUrl}/order/${orderId}`)
      .pipe(map(response => response.data || []));
  }

  getByStatus(status: OutboundStatus): Observable<OutboundShipment[]> {
    return this.http.get<ApiResponse<OutboundShipment[]>>(`${this.apiUrl}/status/${status}`)
      .pipe(map(response => response.data || []));
  }

  getPending(): Observable<OutboundShipment[]> {
    return this.http.get<ApiResponse<OutboundShipment[]>>(`${this.apiUrl}/pending`)
      .pipe(map(response => response.data || []));
  }

  getShipped(): Observable<OutboundShipment[]> {
    return this.http.get<ApiResponse<OutboundShipment[]>>(`${this.apiUrl}/shipped`)
      .pipe(map(response => response.data || []));
  }

  getInTransit(): Observable<OutboundShipment[]> {
    return this.http.get<ApiResponse<OutboundShipment[]>>(`${this.apiUrl}/in-transit`)
      .pipe(map(response => response.data || []));
  }

  create(request: CreateOutboundShipmentRequest): Observable<OutboundShipment> {
    return this.http.post<ApiResponse<OutboundShipment>>(this.apiUrl, request)
      .pipe(map(response => response.data));
  }

  markReadyToShip(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/mark-ready`, {});
  }

  ship(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/ship`, {});
  }

  markInTransit(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/in-transit`, {});
  }

  deliver(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/deliver`, {});
  }

  cancel(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/cancel`, {});
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
