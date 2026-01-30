import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomersService } from '../customers.service';
import { Customer } from '@core/models/customer.model';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { CustomerCreateModalComponent } from '../customer-create-modal/customer-create-modal.component';
import { CustomerEditModalComponent } from '../customer-edit-modal/customer-edit-modal.component';

@Component({
  selector: 'app-customers-list',
  standalone: true,
  imports: [CommonModule, CustomerCreateModalComponent, CustomerEditModalComponent],
  templateUrl: './customers-list.component.html',
  styleUrls: ['./customers-list.component.scss']
})
export class CustomersListComponent implements OnInit {
  private readonly customersService = inject(CustomersService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  customers = signal<Customer[]>([]);
  searchTerm = signal<string>('');
  dateFrom = signal<string>('');
  dateTo = signal<string>('');
  statusFilter = signal<string>('all');
  
  filteredCustomers = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const status = this.statusFilter();
    let result = this.customers();
    
    if (term) {
      result = result.filter(customer => 
        customer.name?.toLowerCase().includes(term) ||
        customer.email?.toLowerCase().includes(term) ||
        customer.phone?.toLowerCase().includes(term) ||
        customer.document?.toLowerCase().includes(term) ||
        customer.city?.toLowerCase().includes(term) ||
        customer.state?.toLowerCase().includes(term)
      );
    }
    
    if (status !== 'all') {
      const isActive = status === 'active';
      result = result.filter(customer => customer.isActive === isActive);
    }
    
    return result;
  });
  
  hasData = computed(() => this.customers().length > 0);
  
  showCreateModal = signal<boolean>(false);
  showEditModal = signal<boolean>(false);
  selectedCustomer = signal<Customer | null>(null);

  ngOnInit(): void {
    this.loadCustomers();
  }

  async loadCustomers(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.customersService.getAll(companyId);
      this.customers.set(response);
    } catch (err) {
      console.error('Erro ao carregar clientes:', err);
    } finally {
      this.loading.set(false);
    }
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase();
  }
  
  openCreateModal(): void {
    this.showCreateModal.set(true);
  }
  
  closeCreateModal(): void {
    this.showCreateModal.set(false);
  }
  
  onCustomerCreated(): void {
    this.loadCustomers();
  }
  
  openEditModal(customer: Customer): void {
    this.selectedCustomer.set(customer);
    this.showEditModal.set(true);
  }
  
  closeEditModal(): void {
    this.showEditModal.set(false);
    this.selectedCustomer.set(null);
  }
  
  onCustomerUpdated(): void {
    this.loadCustomers();
  }
  
  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
  }

  onDateFromChange(event: Event): void {
    this.dateFrom.set((event.target as HTMLInputElement).value);
  }

  onDateToChange(event: Event): void {
    this.dateTo.set((event.target as HTMLInputElement).value);
  }

  onStatusChange(event: Event): void {
    this.statusFilter.set((event.target as HTMLSelectElement).value);
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.dateFrom.set('');
    this.dateTo.set('');
    this.statusFilter.set('all');
  }

  async deleteCustomer(customer: Customer): Promise<void> {
    if (!confirm(`Deseja realmente excluir o cliente "${customer.name}"?`)) return;
    try {
      await this.customersService.delete(customer.id);
      await this.loadCustomers();
    } catch (error) {
      console.error('Erro ao excluir cliente:', error);
      alert(this.i18n.t('common.errors.deleteCustomer'));
    }
  }
}
