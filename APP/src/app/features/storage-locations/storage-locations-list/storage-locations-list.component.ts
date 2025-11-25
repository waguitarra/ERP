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
    const reason = prompt('Motivo do bloqueio:');
    if (!reason) return;
    try {
      await this.storageLocationsService.block(location.id, { reason });
      await this.loadLocations();
    } catch (error) {
      console.error('Erro ao bloquear localização:', error);
      alert('Erro ao bloquear localização');
    }
  }

  async unblockLocation(location: StorageLocation): Promise<void> {
    if (!confirm('Desbloquear esta localização?')) return;
    try {
      await this.storageLocationsService.unblock(location.id);
      await this.loadLocations();
    } catch (error) {
      console.error('Erro ao desbloquear:', error);
      alert('Erro ao desbloquear');
    }
  }

  openCreateModal(): void {
    this.createModal()?.open();
  }

  openEditModal(location: StorageLocation): void {
    this.selectedLocation.set(location);
    setTimeout(() => this.editModal()?.open(), 0);
  }

  async deleteLocation(location: StorageLocation): Promise<void> {
    if (!confirm(`Deseja realmente excluir a localização "${location.code}"?`)) return;
    try {
      await this.storageLocationsService.delete(location.id);
      await this.loadLocations();
    } catch (error) {
      console.error('Erro ao excluir localização:', error);
      alert('Erro ao excluir localização');
    }
  }
}
