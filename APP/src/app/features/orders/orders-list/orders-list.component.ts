import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersService } from '../orders.service';
import { Order } from '@core/models/order.model';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-orders-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders-list.component.html',
  styleUrls: ['./orders-list.component.scss']
})
export class OrdersListComponent implements OnInit {
  private readonly ordersService = inject(OrdersService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  orders = signal<Order[]>([]);
  hasData = computed(() => this.orders().length > 0);

  ngOnInit(): void {
    this.loadOrders();
  }

  async loadOrders(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.ordersService.getAll(companyId);
      this.orders.set(response.data || response || []);
    } catch (err) {
      console.error('Erro ao carregar pedidos:', err);
    } finally {
      this.loading.set(false);
    }
  }

  countByStatus(status: string): number {
    return this.orders().filter(o => o.status === status).length;
  }

  getStatusClass(status: string): string {
    const classes: Record<string, string> = {
      'Pendente': 'bg-amber-50 text-amber-600',
      'Processando': 'bg-blue-50 text-blue-600',
      'Enviado': 'bg-purple-50 text-purple-600',
      'Entregue': 'bg-green-50 text-green-600',
      'Cancelado': 'bg-red-50 text-red-600'
    };
    return classes[status] || 'bg-slate-50 text-slate-600';
  }
}
