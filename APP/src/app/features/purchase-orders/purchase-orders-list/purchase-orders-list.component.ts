import { Component, signal, computed, inject, OnInit, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PurchaseOrdersService, PurchaseOrder } from '@core/services/purchase-orders.service';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { NotificationService } from '@core/services/notification.service';
import { PurchaseOrderCreateModalComponent } from '../purchase-order-create-modal/purchase-order-create-modal.component';

@Component({
  selector: 'app-purchase-orders-list',
  standalone: true,
  imports: [CommonModule, PurchaseOrderCreateModalComponent],
  templateUrl: './purchase-orders-list.component.html'
})
export class PurchaseOrdersListComponent implements OnInit {
  private readonly purchaseOrdersService = inject(PurchaseOrdersService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);
  private readonly notification = inject(NotificationService);

  loading = signal<boolean>(true);
  purchaseOrders = signal<PurchaseOrder[]>([]);
  hasData = computed(() => this.purchaseOrders().length > 0);

  // Computed totals - IGUAL orders-list
  totalOrders = computed(() => this.purchaseOrders().length);
  totalValue = computed(() => this.purchaseOrders().reduce((sum, order) => sum + (order.totalValue || 0), 0));
  
  // Pending orders (Pending, Confirmed)
  pendingOrders = computed(() => this.purchaseOrders().filter(o => 
    o.status === 'Pending' || o.status === 'Confirmed'
  ).length);
  pendingValue = computed(() => this.purchaseOrders()
    .filter(o => o.status === 'Pending' || o.status === 'Confirmed')
    .reduce((sum, order) => sum + (order.totalValue || 0), 0));
  
  // Non-pending orders (all other statuses)
  nonPendingOrders = computed(() => this.purchaseOrders().filter(o => 
    o.status !== 'Pending' && o.status !== 'Confirmed'
  ).length);
  nonPendingValue = computed(() => this.purchaseOrders()
    .filter(o => o.status !== 'Pending' && o.status !== 'Confirmed')
    .reduce((sum, order) => sum + (order.totalValue || 0), 0));

  createModal = viewChild<PurchaseOrderCreateModalComponent>('createModal');

  async ngOnInit(): Promise<void> {
    await this.loadPurchaseOrders();
  }

  async loadPurchaseOrders(): Promise<void> {
    const companyId = this.authService.currentUser()?.companyId;
    if (!companyId) return;

    this.loading.set(true);
    try {
      const data = await this.purchaseOrdersService.getAll(companyId);
      this.purchaseOrders.set(data);
      console.log(`✅ Purchase Orders carregados: ${this.purchaseOrders().length}`);
    } catch (error: any) {
      console.error('❌ Erro ao carregar purchase orders:', error);
      this.notification.error(error.message || this.i18n.t('errors.load_failed'));
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void {
    this.createModal()?.open();
  }

  async deletePurchaseOrder(order: PurchaseOrder): Promise<void> {
    if (!confirm(this.i18n.t('purchase_orders.confirm_delete'))) return;

    this.loading.set(true);
    try {
      await this.purchaseOrdersService.delete(order.id);
      this.notification.success(this.i18n.t('purchase_orders.deleted_success'));
      await this.loadPurchaseOrders();
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.delete_failed'));
    } finally {
      this.loading.set(false);
    }
  }

  getStatusClass(status: string): string {
    const classes: Record<string, string> = {
      'Pending': 'bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400',
      'Confirmed': 'bg-blue-50 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400',
      'InTransit': 'bg-purple-50 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400',
      'Received': 'bg-green-50 dark:bg-green-900/30 text-green-600 dark:text-green-400',
      'Cancelled': 'bg-red-50 dark:bg-red-900/30 text-red-600 dark:text-red-400'
    };
    return classes[status] || 'bg-slate-50 dark:bg-slate-700 text-slate-600 dark:text-slate-400';
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      'Pending': 'Pendente',
      'Confirmed': 'Confirmado',
      'InTransit': 'Em Trânsito',
      'Received': 'Recebido',
      'Cancelled': 'Cancelado'
    };
    return labels[status] || status;
  }
}
