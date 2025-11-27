import { Component, signal, output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomersService } from '@features/customers/customers.service';
import { Customer } from '@core/models/customer.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-customer-selector-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer-selector-modal.component.html',
  styles: [`
    .animate-spin { animation: spin 1s linear infinite; }
    @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
  `]
})
export class CustomerSelectorModalComponent implements OnInit {
  private readonly customersService = inject(CustomersService);
  protected readonly i18n = inject(I18nService);
  
  customerSelected = output<Customer>();
  
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  searchTerm = signal<string>('');
  selectedCustomer = signal<Customer | null>(null);
  customers = signal<Customer[]>([]);
  
  get filteredCustomers(): Customer[] {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.customers();
    
    return this.customers().filter(customer => 
      customer.name.toLowerCase().includes(term) ||
      (customer.email && customer.email.toLowerCase().includes(term)) ||
      (customer.document && customer.document.toLowerCase().includes(term)) ||
      (customer.phone && customer.phone.toLowerCase().includes(term))
    );
  }

  ngOnInit(): void {
    this.loadCustomers();
  }

  async loadCustomers(): Promise<void> {
    console.log('🔵 INICIANDO CARREGAMENTO DE CLIENTES');
    this.loading.set(true);
    try {
      const customers = await this.customersService.getAll();
      console.log('✅ CLIENTES CARREGADOS:', customers.length, customers);
      this.customers.set(customers);
    } catch (error) {
      console.error('❌ ERRO AO CARREGAR CLIENTES:', error);
    } finally {
      this.loading.set(false);
    }
  }

  open(): void {
    console.log('🚀 MODAL ABRINDO - Carregando clientes...');
    this.isOpen.set(true);
    this.searchTerm.set('');
    this.selectedCustomer.set(null);
    this.loadCustomers();
  }

  close(): void {
    this.isOpen.set(false);
  }

  selectCustomer(customer: Customer): void {
    this.selectedCustomer.set(customer);
  }

  confirm(): void {
    const selected = this.selectedCustomer();
    if (selected) {
      this.customerSelected.emit(selected);
      this.close();
    }
  }
}
