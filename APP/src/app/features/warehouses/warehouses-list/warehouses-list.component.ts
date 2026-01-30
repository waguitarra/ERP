import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WarehousesService } from '../warehouses.service';
import { Warehouse } from '@core/models/warehouse.model';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { WarehouseCreateModalComponent } from '../warehouse-create-modal/warehouse-create-modal.component';
import { WarehouseEditModalComponent } from '../warehouse-edit-modal/warehouse-edit-modal.component';

@Component({
  selector: 'app-warehouses-list',
  standalone: true,
  imports: [CommonModule, WarehouseCreateModalComponent, WarehouseEditModalComponent],
  templateUrl: './warehouses-list.component.html',
  styleUrls: ['./warehouses-list.component.scss']
})
export class WarehousesListComponent implements OnInit {
  private readonly warehousesService = inject(WarehousesService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  warehouses = signal<Warehouse[]>([]);
  searchTerm = signal<string>('');
  statusFilter = signal<string>('all');
  
  filteredWarehouses = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const status = this.statusFilter();
    let result = this.warehouses();
    
    if (term) {
      result = result.filter(warehouse =>
        warehouse.name?.toLowerCase().includes(term) ||
        warehouse.code?.toLowerCase().includes(term) ||
        warehouse.city?.toLowerCase().includes(term)
      );
    }
    
    if (status !== 'all') {
      const isActive = status === 'active';
      result = result.filter(warehouse => warehouse.isActive === isActive);
    }
    
    return result;
  });
  
  hasData = computed(() => this.warehouses().length > 0);
  showCreateModal = signal<boolean>(false);
  showEditModal = signal<boolean>(false);
  selectedWarehouse = signal<Warehouse | null>(null);

  ngOnInit(): void { this.loadWarehouses(); }

  async loadWarehouses(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.warehousesService.getAll(companyId);
      this.warehouses.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar armazéns:', err);
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

  clearFilters(): void {
    this.searchTerm.set('');
    this.statusFilter.set('all');
  }

  openCreateModal(): void { this.showCreateModal.set(true); }
  closeCreateModal(): void { this.showCreateModal.set(false); }
  onWarehouseCreated(): void { this.loadWarehouses(); }
  
  openEditModal(warehouse: Warehouse): void {
    this.selectedWarehouse.set(warehouse);
    this.showEditModal.set(true);
  }
  
  closeEditModal(): void {
    this.showEditModal.set(false);
    this.selectedWarehouse.set(null);
  }
  
  onWarehouseUpdated(): void { this.loadWarehouses(); }
  
  async deleteWarehouse(warehouse: Warehouse): Promise<void> {
    if (!confirm(`Deseja excluir o armazém "${warehouse.name}"?`)) return;
    try {
      await this.warehousesService.delete(warehouse.id);
      await this.loadWarehouses();
    } catch (error) {
      console.error('Erro ao excluir armazém:', error);
      alert(this.i18n.t('common.errors.deleteWarehouse'));
    }
  }
}
