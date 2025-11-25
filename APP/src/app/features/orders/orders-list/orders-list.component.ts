import { Component, signal, computed, inject, OnInit, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersService } from '../orders.service';
import { Order } from '@core/models/order.model';
import { OrderStatus } from '@core/models/enums';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { OrderCreateModalComponent } from '../order-create-modal/order-create-modal.component';
import { OrderEditModalComponent } from '../order-edit-modal/order-edit-modal.component';
import { CompaniesService } from '@features/companies/companies.service';

@Component({
  selector: 'app-orders-list',
  standalone: true,
  imports: [CommonModule, OrderCreateModalComponent, OrderEditModalComponent],
  templateUrl: './orders-list.component.html',
  styleUrls: ['./orders-list.component.scss']
})
export class OrdersListComponent implements OnInit {
  private readonly ordersService = inject(OrdersService);
  private readonly authService = inject(AuthService);
  private readonly companiesService = inject(CompaniesService);
  protected readonly i18n = inject(I18nService);

  // Expor enum para uso no template
  readonly OrderStatus = OrderStatus;

  loading = signal<boolean>(true);
  orders = signal<Order[]>([]);
  selectedOrder = signal<Order | null>(null);
  hasData = computed(() => this.orders().length > 0);

  createModal = viewChild<OrderCreateModalComponent>('createModal');
  editModal = viewChild<OrderEditModalComponent>('editModal');

  ngOnInit(): void {
    this.loadOrders();
  }

  async loadOrders(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      let companyId = user?.companyId;

      // Se admin sem empresa, pega primeira empresa disponível
      if (!companyId) {
        console.warn('⚠️ Usuário sem companyId. Buscando primeira empresa...');
        const companiesResp = await this.companiesService.getAll();
        const companies = companiesResp.data || [];
        if (companies.length > 0) {
          companyId = companies[0].id;
          console.log(`✅ Usando empresa: ${companies[0].name} (${companyId})`);
        }
      }

      const response = await this.ordersService.getAll(companyId || undefined);
      this.orders.set(response.data || response || []);
      console.log(`✅ Orders carregados: ${this.orders().length}`);
    } catch (err) {
      console.error('❌ Erro ao carregar pedidos:', err);
    } finally {
      this.loading.set(false);
    }
  }

  countByStatus(status: OrderStatus): number {
    return this.orders().filter((o: Order) => o.status === status).length;
  }

  getStatusClass(status: OrderStatus): string {
    const classes: Record<number, string> = {
      [OrderStatus.Draft]: 'bg-slate-50 dark:bg-slate-700 text-slate-600 dark:text-slate-400',
      [OrderStatus.Pending]: 'bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400',
      [OrderStatus.Confirmed]: 'bg-blue-50 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400',
      [OrderStatus.InProgress]: 'bg-purple-50 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400',
      [OrderStatus.Shipped]: 'bg-cyan-50 dark:bg-cyan-900/30 text-cyan-600 dark:text-cyan-400',
      [OrderStatus.Delivered]: 'bg-green-50 dark:bg-green-900/30 text-green-600 dark:text-green-400',
      [OrderStatus.Cancelled]: 'bg-red-50 dark:bg-red-900/30 text-red-600 dark:text-red-400',
      [OrderStatus.OnHold]: 'bg-orange-50 dark:bg-orange-900/30 text-orange-600 dark:text-orange-400'
    };
    return classes[status] || 'bg-slate-50 dark:bg-slate-700 text-slate-600 dark:text-slate-400';
  }

  getStatusLabel(status: OrderStatus): string {
    const labels: Record<number, string> = {
      [OrderStatus.Draft]: 'Rascunho',
      [OrderStatus.Pending]: 'Pendente',
      [OrderStatus.Confirmed]: 'Confirmado',
      [OrderStatus.InProgress]: 'Em Andamento',
      [OrderStatus.PartiallyFulfilled]: 'Parcialmente Atendido',
      [OrderStatus.Fulfilled]: 'Atendido',
      [OrderStatus.Shipped]: 'Enviado',
      [OrderStatus.Delivered]: 'Entregue',
      [OrderStatus.Cancelled]: 'Cancelado',
      [OrderStatus.OnHold]: 'Em Espera'
    };
    return labels[status] || 'Desconhecido';
  }

  openCreateModal(): void {
    this.createModal()?.open();
  }

  openEditModal(order: Order): void {
    this.selectedOrder.set(order);
    setTimeout(() => this.editModal()?.open(), 0);
  }

  async deleteOrder(order: Order): Promise<void> {
    if (!confirm(`Deseja realmente excluir o pedido "${order.orderNumber}"?`)) return;
    try {
      await this.ordersService.delete(order.id);
      await this.loadOrders();
    } catch (error) {
      console.error('Erro ao excluir pedido:', error);
      alert('Erro ao excluir pedido');
    }
  }
}
