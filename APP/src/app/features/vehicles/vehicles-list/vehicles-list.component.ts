import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { VehiclesService } from '../vehicles.service';
import { Vehicle, VehicleStatus } from '@core/models/vehicle.model';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { VehicleCreateModalComponent } from '../vehicle-create-modal/vehicle-create-modal.component';
import { VehicleEditModalComponent } from '../vehicle-edit-modal/vehicle-edit-modal.component';

@Component({
  selector: 'app-vehicles-list',
  standalone: true,
  imports: [CommonModule, VehicleCreateModalComponent, VehicleEditModalComponent],
  templateUrl: './vehicles-list.component.html',
  styleUrls: ['./vehicles-list.component.scss']
})
export class VehiclesListComponent implements OnInit {
  private readonly vehiclesService = inject(VehiclesService);
  private readonly authService = inject(AuthService);
  private readonly sanitizer = inject(DomSanitizer);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  vehicles = signal<Vehicle[]>([]);
  searchTerm = signal<string>('');
  statusFilter = signal<string>('all');
  trackingFilter = signal<string>('all');
  
  // Map view state
  showMapView = signal<boolean>(false);
  selectedVehicleForMap = signal<Vehicle | null>(null);
  
  filteredVehicles = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const status = this.statusFilter();
    const tracking = this.trackingFilter();
    let result = this.vehicles();
    
    if (term) {
      result = result.filter(vehicle =>
        vehicle.licensePlate?.toLowerCase().includes(term) ||
        vehicle.model?.toLowerCase().includes(term) ||
        vehicle.brand?.toLowerCase().includes(term) ||
        vehicle.vehicleType?.toLowerCase().includes(term) ||
        vehicle.driverName?.toLowerCase().includes(term)
      );
    }
    
    if (status !== 'all') {
      result = result.filter(vehicle => vehicle.status === parseInt(status));
    }
    
    if (tracking === 'enabled') {
      result = result.filter(vehicle => vehicle.trackingEnabled);
    } else if (tracking === 'disabled') {
      result = result.filter(vehicle => !vehicle.trackingEnabled);
    }
    
    return result;
  });
  
  hasData = computed(() => this.vehicles().length > 0);
  showCreateModal = signal<boolean>(false);
  showEditModal = signal<boolean>(false);
  selectedVehicle = signal<Vehicle | null>(null);

  ngOnInit(): void { this.loadVehicles(); }

  async loadVehicles(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.vehiclesService.getAll(companyId);
      this.vehicles.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar veículos:', err);
    } finally {
      this.loading.set(false);
    }
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
  }

  onStatusChange(event: Event): void {
    this.statusFilter.set((event.target as HTMLSelectElement).value);
  }

  onTrackingChange(event: Event): void {
    this.trackingFilter.set((event.target as HTMLSelectElement).value);
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.statusFilter.set('all');
    this.trackingFilter.set('all');
  }

  openCreateModal(): void { this.showCreateModal.set(true); }
  closeCreateModal(): void { this.showCreateModal.set(false); }
  onVehicleCreated(): void { this.loadVehicles(); }
  
  openEditModal(vehicle: Vehicle): void {
    this.selectedVehicle.set(vehicle);
    this.showEditModal.set(true);
  }
  
  closeEditModal(): void {
    this.showEditModal.set(false);
    this.selectedVehicle.set(null);
  }
  
  onVehicleUpdated(): void { this.loadVehicles(); }
  
  async deleteVehicle(vehicle: Vehicle): Promise<void> {
    if (!confirm(`Deseja realmente excluir o veículo "${vehicle.licensePlate}"?`)) return;
    try {
      await this.vehiclesService.delete(vehicle.id);
      await this.loadVehicles();
    } catch (error) {
      console.error('Erro ao excluir veículo:', error);
      alert(this.i18n.t('common.errors.deleteVehicle'));
    }
  }

  async toggleTracking(vehicle: Vehicle): Promise<void> {
    try {
      if (vehicle.trackingEnabled) {
        await this.vehiclesService.disableTracking(vehicle.id);
      } else {
        await this.vehiclesService.enableTracking(vehicle.id);
      }
      await this.loadVehicles();
    } catch (error) {
      console.error('Erro ao alterar rastreamento:', error);
      alert(this.i18n.t('common.errors.toggleTracking'));
    }
  }

  openMapView(vehicle: Vehicle): void {
    this.selectedVehicleForMap.set(vehicle);
    this.showMapView.set(true);
  }

  closeMapView(): void {
    this.showMapView.set(false);
    this.selectedVehicleForMap.set(null);
  }

  getStatusClass(status: number): string {
    switch (status) {
      case VehicleStatus.Available:
        return 'bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400';
      case VehicleStatus.InTransit:
        return 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400';
      case VehicleStatus.Maintenance:
        return 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-600 dark:text-yellow-400';
      case VehicleStatus.Inactive:
        return 'bg-slate-100 dark:bg-slate-900/30 text-slate-600 dark:text-slate-400';
      default:
        return 'bg-slate-100 dark:bg-slate-900/30 text-slate-600 dark:text-slate-400';
    }
  }

  getWhatsAppLink(phone?: string): string {
    if (!phone) return '';
    const cleanPhone = phone.replace(/\D/g, '');
    return `https://wa.me/55${cleanPhone}`;
  }

  formatLastUpdate(dateStr?: string): string {
    if (!dateStr) return '-';
    const date = new Date(dateStr);
    return date.toLocaleString('pt-BR');
  }

  getGoogleMapsUrl(vehicle: Vehicle): SafeResourceUrl {
    const lat = vehicle.lastLatitude || 40.4168; // Default: Madrid
    const lng = vehicle.lastLongitude || -3.7038;
    const zoom = 16;
    
    // Use Google Maps embed API com marcador no ponto do veículo
    // Mostra mapa de satélite/rua com o marcador na posição
    const mapUrl = `https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1500!2d${lng}!3d${lat}!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zM${Math.abs(lat).toFixed(4)}!5e0!3m2!1spt-BR!2sbr!4v${Date.now()}`;
    
    return this.sanitizer.bypassSecurityTrustResourceUrl(mapUrl);
  }
}
