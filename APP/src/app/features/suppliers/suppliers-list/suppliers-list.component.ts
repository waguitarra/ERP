import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SuppliersService } from '../suppliers.service';
import { Supplier } from '@core/models/supplier.model';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { SupplierCreateModalComponent } from '../supplier-create-modal/supplier-create-modal.component';
import { SupplierEditModalComponent } from '../supplier-edit-modal/supplier-edit-modal.component';

@Component({
  selector: 'app-suppliers-list',
  standalone: true,
  imports: [CommonModule, SupplierCreateModalComponent, SupplierEditModalComponent],
  templateUrl: './suppliers-list.component.html',
  styleUrls: ['./suppliers-list.component.scss']
})
export class SuppliersListComponent implements OnInit {
  private readonly suppliersService = inject(SuppliersService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  suppliers = signal<Supplier[]>([]);
  searchTerm = signal<string>('');
  statusFilter = signal<string>('all');
  
  filteredSuppliers = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const status = this.statusFilter();
    let result = this.suppliers();
    
    if (term) {
      result = result.filter(supplier => 
        supplier.name?.toLowerCase().includes(term) ||
        supplier.email?.toLowerCase().includes(term) ||
        supplier.phone?.toLowerCase().includes(term) ||
        supplier.document?.toLowerCase().includes(term) ||
        supplier.city?.toLowerCase().includes(term)
      );
    }
    
    if (status !== 'all') {
      const isActive = status === 'active';
      result = result.filter(supplier => supplier.isActive === isActive);
    }
    
    return result;
  });
  
  hasData = computed(() => this.suppliers().length > 0);
  
  showCreateModal = signal<boolean>(false);
  showEditModal = signal<boolean>(false);
  selectedSupplier = signal<Supplier | null>(null);

  ngOnInit(): void {
    this.loadSuppliers();
  }

  async loadSuppliers(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.suppliersService.getAll(companyId);
      this.suppliers.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar fornecedores:', err);
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void {
    this.showCreateModal.set(true);
  }
  
  closeCreateModal(): void {
    this.showCreateModal.set(false);
  }
  
  onSupplierCreated(): void {
    this.loadSuppliers();
  }
  
  openEditModal(supplier: Supplier): void {
    this.selectedSupplier.set(supplier);
    this.showEditModal.set(true);
  }
  
  closeEditModal(): void {
    this.showEditModal.set(false);
    this.selectedSupplier.set(null);
  }
  
  onSupplierUpdated(): void {
    this.loadSuppliers();
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

  async deleteSupplier(supplier: Supplier): Promise<void> {
    if (!confirm(`Deseja realmente excluir o fornecedor "${supplier.name}"?`)) return;
    try {
      await this.suppliersService.delete(supplier.id);
      await this.loadSuppliers();
    } catch (error) {
      console.error('Erro ao excluir fornecedor:', error);
      alert(this.i18n.t('common.errors.deleteSupplier'));
    }
  }
}
