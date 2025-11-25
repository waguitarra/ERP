import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VehiclesService } from '../vehicles.service';
import { Vehicle } from '@core/models/vehicle.model';
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
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  vehicles = signal<Vehicle[]>([]);
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
    if (!confirm(`Deseja realmente excluir o veículo "${vehicle.plateNumber}"?`)) return;
    try {
      await this.vehiclesService.delete(vehicle.id);
      await this.loadVehicles();
    } catch (error) {
      console.error('Erro ao excluir veículo:', error);
      alert('Erro ao excluir veículo');
    }
  }
}
