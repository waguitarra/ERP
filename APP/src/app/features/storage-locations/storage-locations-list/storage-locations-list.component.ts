import { Component, signal, computed, inject, OnInit, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StorageLocationsService } from '../storage-locations.service';
import { StorageLocation } from '@core/models/storage-location.model';
import { I18nService } from '@core/services/i18n.service';
import { StorageLocationCreateModalComponent } from '../storage-location-create-modal/storage-location-create-modal.component';
import { StorageLocationEditModalComponent } from '../storage-location-edit-modal/storage-location-edit-modal.component';

@Component({
  selector: 'app-storage-locations-list',
  standalone: true,
  imports: [CommonModule, StorageLocationCreateModalComponent, StorageLocationEditModalComponent],
  templateUrl: './storage-locations-list.component.html',
  styleUrls: ['./storage-locations-list.component.scss']
})
export class StorageLocationsListComponent implements OnInit {
  private readonly storageLocationsService = inject(StorageLocationsService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  locations = signal<StorageLocation[]>([]);
  selectedLocation = signal<StorageLocation | null>(null);
  searchTerm = signal<string>('');
  statusFilter = signal<string>('all');
  
  filteredLocations = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const status = this.statusFilter();
    let result = this.locations();
    
    if (term) {
      result = result.filter(loc =>
        loc.code?.toLowerCase().includes(term) ||
        loc.aisle?.toLowerCase().includes(term) ||
        loc.rack?.toLowerCase().includes(term) ||
        loc.locationType?.toLowerCase().includes(term)
      );
    }
    
    if (status !== 'all') {
      if (status === 'blocked') {
        result = result.filter(loc => loc.isBlocked === true);
      } else if (status === 'active') {
        result = result.filter(loc => loc.isActive === true && !loc.isBlocked);
      } else if (status === 'inactive') {
        result = result.filter(loc => loc.isActive === false);
      }
    }
    
    return result;
  });
  
  hasData = computed(() => this.locations().length > 0);

  createModal = viewChild<StorageLocationCreateModalComponent>('createModal');
  editModal = viewChild<StorageLocationEditModalComponent>('editModal');

  ngOnInit(): void {
    this.loadLocations();
  }

  async loadLocations(): Promise<void> {
    this.loading.set(true);
    try {
      const response = await this.storageLocationsService.getAll();
      this.locations.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar localizações:', err);
    } finally {
      this.loading.set(false);
    }
  }

  async blockLocation(location: StorageLocation): Promise<void> {
    const reason = prompt(this.i18n.t('common.prompts.blockReason'));
    if (!reason) return;
    try {
      await this.storageLocationsService.block(location.id, { reason });
      await this.loadLocations();
    } catch (error) {
      console.error('Erro ao bloquear localização:', error);
      alert(this.i18n.t('common.errors.blockLocation'));
    }
  }

  async unblockLocation(location: StorageLocation): Promise<void> {
    if (!confirm(this.i18n.t('common.confirms.unlockLocation'))) return;
    try {
      await this.storageLocationsService.unblock(location.id);
      await this.loadLocations();
    } catch (error) {
      console.error('Erro ao desbloquear:', error);
      alert(this.i18n.t('common.errors.unblockLocation'));
    }
  }

  onSearch(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  onStatusChange(event: Event): void {
    this.statusFilter.set((event.target as HTMLSelectElement).value);
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.statusFilter.set('all');
  }

  openCreateModal(): void {
    this.createModal()?.open();
  }

  openEditModal(location: StorageLocation): void {
    this.selectedLocation.set(location);
    setTimeout(() => this.editModal()?.open(), 0);
  }

  async deleteLocation(location: StorageLocation): Promise<void> {
    if (!confirm(this.i18n.t('common.confirms.deleteLocation'))) return;
    try {
      await this.storageLocationsService.delete(location.id);
      await this.loadLocations();
    } catch (error) {
      console.error('Erro ao excluir localização:', error);
      alert(this.i18n.t('common.errors.deleteLocation'));
    }
  }
}
