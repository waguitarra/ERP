import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { 
  InboundShipment, 
  InboundShipmentListResponse, 
  InboundShipmentResponse,
  CreateInboundShipmentRequest 
} from '@core/models/inbound-shipment.model';

@Injectable({
  providedIn: 'root'
})
export class InboundShipmentsService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/inbound-shipments';

  getAll(): Promise<InboundShipmentListResponse> {
    return this.api.get<InboundShipmentListResponse>(this.endpoint);
  }

  getById(id: string): Promise<InboundShipmentResponse> {
    return this.api.get<InboundShipmentResponse>(`${this.endpoint}/${id}`);
  }

  getScheduled(): Promise<InboundShipmentListResponse> {
    return this.api.get<InboundShipmentListResponse>(`${this.endpoint}/scheduled`);
  }

  getInProgress(): Promise<InboundShipmentListResponse> {
    return this.api.get<InboundShipmentListResponse>(`${this.endpoint}/in-progress`);
  }

  getByStatus(status: number): Promise<InboundShipmentListResponse> {
    return this.api.get<InboundShipmentListResponse>(`${this.endpoint}/status/${status}`);
  }

  getByCompany(companyId: string): Promise<InboundShipmentListResponse> {
    return this.api.get<InboundShipmentListResponse>(`${this.endpoint}/company/${companyId}`);
  }

  create(request: CreateInboundShipmentRequest): Promise<InboundShipmentResponse> {
    return this.api.post<InboundShipmentResponse>(this.endpoint, request);
  }

  receive(shipmentId: string, receivedBy: string): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${shipmentId}/receive`, { receivedBy });
  }

  complete(shipmentId: string): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${shipmentId}/complete`, {});
  }

  cancel(shipmentId: string): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${shipmentId}/cancel`, {});
  }

  delete(shipmentId: string): Promise<any> {
    return this.api.delete<any>(`${this.endpoint}/${shipmentId}`);
  }
}
