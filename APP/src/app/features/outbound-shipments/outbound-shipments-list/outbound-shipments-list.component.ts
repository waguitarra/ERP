import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { OutboundShipmentsService } from '../outbound-shipments.service';
import { OutboundShipment, OutboundStatus } from '../../../core/models/outbound-shipment.model';

@Component({
  selector: 'app-outbound-shipments-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './outbound-shipments-list.component.html',
  styleUrls: ['./outbound-shipments-list.component.scss']
})
export class OutboundShipmentsListComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly shipmentsService = inject(OutboundShipmentsService);

  // Data
  shipments = signal<OutboundShipment[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  // Filters
  searchTerm = signal('');
  statusFilter = signal<number | null>(null);
  
  // Status enum for template
  readonly OutboundStatus = OutboundStatus;
  
  // Computed stats
  stats = computed(() => {
    const all = this.shipments();
    return {
      total: all.length,
      pending: all.filter(s => s.status === OutboundStatus.Pending).length,
      readyToShip: all.filter(s => s.status === OutboundStatus.ReadyToShip).length,
      shipped: all.filter(s => s.status === OutboundStatus.Shipped).length,
      inTransit: all.filter(s => s.status === OutboundStatus.InTransit).length,
      delivered: all.filter(s => s.status === OutboundStatus.Delivered).length,
      cancelled: all.filter(s => s.status === OutboundStatus.Cancelled).length
    };
  });
  
  // Filtered shipments
  filteredShipments = computed(() => {
    let result = this.shipments();
    
    // Apply status filter
    const status = this.statusFilter();
    if (status !== null) {
      result = result.filter(s => s.status === status);
    }
    
    // Apply search
    const search = this.searchTerm().toLowerCase();
    if (search) {
      result = result.filter(s => 
        s.shipmentNumber.toLowerCase().includes(search) ||
        s.orderNumber?.toLowerCase().includes(search) ||
        s.trackingNumber?.toLowerCase().includes(search) ||
        s.deliveryAddress?.toLowerCase().includes(search)
      );
    }
    
    return result;
  });

  ngOnInit(): void {
    this.loadShipments();
  }

  loadShipments(): void {
    this.loading.set(true);
    this.error.set(null);

    this.shipmentsService.getAll().subscribe({
      next: (data) => {
        this.shipments.set(data || []);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(this.i18n.t('outboundShipments.errors.loadFailed'));
        this.loading.set(false);
        console.error('Error loading outbound shipments:', err);
      }
    });
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
  }

  filterByStatus(status: number | null): void {
    this.statusFilter.set(status);
  }

  getStatusLabel(status: number): string {
    const keys: Record<number, string> = {
      [OutboundStatus.Pending]: 'outboundShipments.status.pending',
      [OutboundStatus.ReadyToShip]: 'outboundShipments.status.readyToShip',
      [OutboundStatus.Shipped]: 'outboundShipments.status.shipped',
      [OutboundStatus.InTransit]: 'outboundShipments.status.inTransit',
      [OutboundStatus.Delivered]: 'outboundShipments.status.delivered',
      [OutboundStatus.Cancelled]: 'outboundShipments.status.cancelled'
    };

    const key = keys[status];
    return key ? this.i18n.t(key) : this.i18n.t('outboundShipments.status.unknown');
  }

  getStatusClass(status: number): string {
    switch (status) {
      case OutboundStatus.Pending: return 'badge-warning';
      case OutboundStatus.ReadyToShip: return 'badge-info';
      case OutboundStatus.Shipped: return 'badge-primary';
      case OutboundStatus.InTransit: return 'badge-accent';
      case OutboundStatus.Delivered: return 'badge-success';
      case OutboundStatus.Cancelled: return 'badge-error';
      default: return 'badge-ghost';
    }
  }

  markReadyToShip(shipment: OutboundShipment): void {
    this.shipmentsService.markReadyToShip(shipment.id).subscribe({
      next: () => this.loadShipments(),
      error: (err) => console.error('Error marking ready to ship:', err)
    });
  }

  shipShipment(shipment: OutboundShipment): void {
    this.shipmentsService.ship(shipment.id).subscribe({
      next: () => this.loadShipments(),
      error: (err) => console.error('Error shipping:', err)
    });
  }

  markInTransit(shipment: OutboundShipment): void {
    this.shipmentsService.markInTransit(shipment.id).subscribe({
      next: () => this.loadShipments(),
      error: (err) => console.error('Error marking in transit:', err)
    });
  }

  deliverShipment(shipment: OutboundShipment): void {
    this.shipmentsService.deliver(shipment.id).subscribe({
      next: () => this.loadShipments(),
      error: (err) => console.error('Error delivering:', err)
    });
  }

  cancelShipment(shipment: OutboundShipment): void {
    if (confirm(this.i18n.t('outboundShipments.confirm.cancel'))) {
      this.shipmentsService.cancel(shipment.id).subscribe({
        next: () => this.loadShipments(),
        error: (err) => console.error('Error cancelling:', err)
      });
    }
  }

  deleteShipment(shipment: OutboundShipment): void {
    if (confirm(this.i18n.t('outboundShipments.confirm.delete'))) {
      this.shipmentsService.delete(shipment.id).subscribe({
        next: () => this.loadShipments(),
        error: (err) => console.error('Error deleting:', err)
      });
    }
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    const lang = this.i18n.currentLanguage();

    return new Date(date).toLocaleDateString(lang, {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
