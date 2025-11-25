import { Component, signal, output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrdersService } from '@features/orders/orders.service';
import { Order } from '@core/models/order.model';

@Component({
  selector: 'app-order-selector-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './order-selector-modal.component.html',
  styles: [`
    .animate-spin { animation: spin 1s linear infinite; }
    @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
  `]
})
export class OrderSelectorModalComponent implements OnInit {
  private readonly ordersService = inject(OrdersService);
  
  orderSelected = output<Order>();
  
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  searchTerm = signal<string>('');
  selectedOrder = signal<Order | null>(null);
  orders = signal<Order[]>([]);
  
  get filteredOrders(): Order[] {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.orders();
    
    return this.orders().filter(order => 
      order.id.toLowerCase().includes(term) ||
      order.orderNumber.toLowerCase().includes(term) ||
      (order.customerName && order.customerName.toLowerCase().includes(term))
    );
  }

  ngOnInit(): void {
    this.loadOrders();
  }

  async loadOrders(): Promise<void> {
    this.loading.set(true);
    try {
      const orders = await this.ordersService.getAll();
      this.orders.set(orders);
    } catch (error) {
      console.error('Erro ao carregar pedidos:', error);
    } finally {
      this.loading.set(false);
    }
  }

  open(): void {
    this.isOpen.set(true);
    this.searchTerm.set('');
    this.selectedOrder.set(null);
    this.loadOrders();
  }

  close(): void {
    this.isOpen.set(false);
  }

  selectOrder(order: Order): void {
    this.selectedOrder.set(order);
  }

  confirm(): void {
    const selected = this.selectedOrder();
    if (selected) {
      this.orderSelected.emit(selected);
      this.close();
    }
  }

  getStatusLabel(status: number): string {
    const labels: Record<number, string> = {
      0: 'Rascunho',
      1: 'Pendente',
      2: 'Confirmado',
      3: 'Em Processamento',
      4: 'Enviado',
      5: 'Entregue',
      6: 'Cancelado'
    };
    return labels[status] || 'Desconhecido';
  }

  getStatusColor(status: number): string {
    const colors: Record<number, string> = {
      0: 'bg-gray-100 text-gray-800',
      1: 'bg-yellow-100 text-yellow-800',
      2: 'bg-blue-100 text-blue-800',
      3: 'bg-purple-100 text-purple-800',
      4: 'bg-indigo-100 text-indigo-800',
      5: 'bg-green-100 text-green-800',
      6: 'bg-red-100 text-red-800'
    };
    return colors[status] || 'bg-gray-100 text-gray-800';
  }

  getPriorityLabel(priority: number): string {
    const labels: Record<number, string> = {
      0: 'Baixa',
      1: 'Normal',
      2: 'Alta',
      3: 'Urgente'
    };
    return labels[priority] || 'Normal';
  }
}
