import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DriversService } from '../drivers.service';
import { Driver } from '@core/models/driver.model';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { DriverCreateModalComponent } from '../driver-create-modal/driver-create-modal.component';
import { DriverEditModalComponent } from '../driver-edit-modal/driver-edit-modal.component';

@Component({
  selector: 'app-drivers-list',
  standalone: true,
  imports: [CommonModule, DriverCreateModalComponent, DriverEditModalComponent],
  templateUrl: './drivers-list.component.html',
  styleUrls: ['./drivers-list.component.scss']
})
export class DriversListComponent implements OnInit {
  private readonly driversService = inject(DriversService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  drivers = signal<Driver[]>([]);
  hasData = computed(() => this.drivers().length > 0);
  showCreateModal = signal<boolean>(false);
  showEditModal = signal<boolean>(false);
  selectedDriver = signal<Driver | null>(null);

  ngOnInit(): void { this.loadDrivers(); }

  async loadDrivers(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.driversService.getAll(companyId);
      this.drivers.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar motoristas:', err);
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void { this.showCreateModal.set(true); }
  closeCreateModal(): void { this.showCreateModal.set(false); }
  onDriverCreated(): void { this.loadDrivers(); }
  
  openEditModal(driver: Driver): void {
    this.selectedDriver.set(driver);
    this.showEditModal.set(true);
  }
  
  closeEditModal(): void {
    this.showEditModal.set(false);
    this.selectedDriver.set(null);
  }
  
  onDriverUpdated(): void { this.loadDrivers(); }
  
  async deleteDriver(driver: Driver): Promise<void> {
    if (!confirm(`Deseja excluir o motorista "${driver.name}"?`)) return;
    try {
      await this.driversService.delete(driver.id);
      await this.loadDrivers();
    } catch (error) {
      console.error('Erro ao excluir motorista:', error);
      alert('Erro ao excluir motorista');
    }
  }
}
