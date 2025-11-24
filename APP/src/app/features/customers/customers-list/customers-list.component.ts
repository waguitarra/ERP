import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomersService } from '../customers.service';
import { Customer } from '@core/models/customer.model';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-customers-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './customers-list.component.html',
  styleUrls: ['./customers-list.component.scss']
})
export class CustomersListComponent implements OnInit {
  private readonly customersService = inject(CustomersService);
  private readonly authService = inject(AuthService);

  loading = signal<boolean>(true);
  customers = signal<Customer[]>([]);
  hasData = computed(() => this.customers().length > 0);

  ngOnInit(): void {
    this.loadCustomers();
  }

  async loadCustomers(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.customersService.getAll(companyId);
      this.customers.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar clientes:', err);
    } finally {
      this.loading.set(false);
    }
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase();
  }
}
