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
  
  async deleteCustomer(customer: Customer): Promise<void> {
    if (!confirm(`Deseja realmente excluir o cliente "${customer.name}"?`)) return;
    try {
      await this.customersService.delete(customer.id);
      await this.loadCustomers();
    } catch (error) {
      console.error('Erro ao excluir cliente:', error);
      alert('Erro ao excluir cliente');
    }
  }
}
