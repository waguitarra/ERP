import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InboundShipmentsService } from '../inbound-shipments.service';
import { InboundShipment, InboundShipmentStatus } from '@core/models/inbound-shipment.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-inbound-shipments-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inbound-shipments-list.component.html',
  styleUrls: ['./inbound-shipments-list.component.scss']
})
export class InboundShipmentsListComponent implements OnInit {
  private readonly inboundService = inject(InboundShipmentsService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  shipments = signal<InboundShipment[]>([]);
  selectedFilter = signal<string>('all');
  searchTerm = signal<string>('');
  
  hasData = computed(() => this.filteredShipments().length > 0);
  
  filteredShipments = computed(() => {
    let result = this.shipments();
    const search = this.searchTerm().toLowerCase();
    
    if (search) {
      result = result.filter(s => 
        s.shipmentNumber.toLowerCase().includes(search) ||
        s.orderNumber?.toLowerCase().includes(search) ||
        s.supplierName?.toLowerCase().includes(search) ||
        s.asnNumber?.toLowerCase().includes(search)
      );
    }
    
    return result;
  });

  // Statistics
  stats = computed(() => {
    const all = this.shipments();
    return {
      total: all.length,
      scheduled: all.filter(s => s.status === InboundShipmentStatus.Scheduled).length,
      inProgress: all.filter(s => s.status === InboundShipmentStatus.InProgress).length,
      completed: all.filter(s => s.status === InboundShipmentStatus.Completed).length,
      cancelled: all.filter(s => s.status === InboundShipmentStatus.Cancelled).length
    };
  });

  ngOnInit(): void {
    this.loadShipments();
  }

  async loadShipments(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      let response;
      switch (this.selectedFilter()) {
        case 'scheduled':
          response = await this.inboundService.getScheduled();
          break;
        case 'in-progress':
          response = await this.inboundService.getInProgress();
          break;
        case 'completed':
          response = await this.inboundService.getByStatus(InboundShipmentStatus.Completed);
          break;
        default:
          response = await this.inboundService.getAll();
      }
      this.shipments.set(response.data || []);
    } catch (err: any) {
      this.error.set(err.message || this.i18n.t('inboundShipments.errors.loadFailed'));
      console.error('Error loading inbound shipments:', err);
    } finally {
      this.loading.set(false);
    }
  }

  onFilterChange(filter: string): void {
    this.selectedFilter.set(filter);
    this.loadShipments();
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
  }

  async receiveShipment(shipment: InboundShipment): Promise<void> {
    try {
      // TODO: Get current user ID
      const currentUserId = '5d139bb8-025f-4820-8364-57134a9eceb8';
      await this.inboundService.receive(shipment.id, currentUserId);
      await this.loadShipments();
    } catch (error) {
      console.error('Erro ao receber shipment:', error);
    }
  }

  async completeShipment(shipment: InboundShipment): Promise<void> {
    try {
      await this.inboundService.complete(shipment.id);
      await this.loadShipments();
    } catch (error) {
      console.error('Erro ao completar shipment:', error);
    }
  }

  async cancelShipment(shipment: InboundShipment): Promise<void> {
    if (!confirm(this.i18n.t('inboundShipments.confirm.cancel'))) return;
    
    try {
      await this.inboundService.cancel(shipment.id);
      await this.loadShipments();
    } catch (error) {
      console.error('Erro ao cancelar shipment:', error);
    }
  }

  async deleteShipment(shipment: InboundShipment): Promise<void> {
    if (!confirm(this.i18n.t('inboundShipments.confirm.delete'))) return;
    
    try {
      await this.inboundService.delete(shipment.id);
      await this.loadShipments();
    } catch (error) {
      console.error('Erro ao excluir shipment:', error);
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case InboundShipmentStatus.Scheduled:
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400';
      case InboundShipmentStatus.InProgress:
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400';
      case InboundShipmentStatus.Completed:
        return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400';
      case InboundShipmentStatus.Cancelled:
        return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400';
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400';
    }
  }

  getStatusLabel(status: number): string {
    const keys: Record<number, string> = {
      [InboundShipmentStatus.Scheduled]: 'inboundShipments.status.scheduled',
      [InboundShipmentStatus.InProgress]: 'inboundShipments.status.inProgress',
      [InboundShipmentStatus.Completed]: 'inboundShipments.status.completed',
      [InboundShipmentStatus.Cancelled]: 'inboundShipments.status.cancelled'
    };

    const key = keys[status];
    return key ? this.i18n.t(key) : this.i18n.t('inboundShipments.status.unknown');
  }

  canReceive(shipment: InboundShipment): boolean {
    return shipment.status === InboundShipmentStatus.Scheduled;
  }

  canComplete(shipment: InboundShipment): boolean {
    return shipment.status === InboundShipmentStatus.InProgress;
  }

  canCancel(shipment: InboundShipment): boolean {
    return shipment.status !== InboundShipmentStatus.Completed && shipment.status !== InboundShipmentStatus.Cancelled;
  }

  formatDate(dateStr: string | undefined): string {
    if (!dateStr) return '-';
    const lang = this.i18n.currentLanguage();
    return new Date(dateStr).toLocaleDateString(lang, {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getProgress(shipment: InboundShipment): number {
    if (shipment.totalQuantityExpected === 0) return 0;
    return Math.round((shipment.totalQuantityReceived / shipment.totalQuantityExpected) * 100);
  }
}
