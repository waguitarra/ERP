import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { I18nService } from '@core/services/i18n.service';
import { DashboardService, DashboardStats, RecentOrder } from '@core/services/dashboard.service';

interface StatCard {
  title: string;
  value: number;
  displayValue: string;
  valueLabel?: string;
  secondaryValue?: string;
  secondaryLabel?: string;
  trend?: number[];
  route: string;
  icon: string;
  color: string;
  bgColor: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly dashboardService = inject(DashboardService);
  private readonly router = inject(Router);

  // Signals for reactive data
  loading = signal(true);
  statsData = signal<DashboardStats | null>(null);
  recentOrders = signal<RecentOrder[]>([]);
  recentPurchaseOrders = signal<any[]>([]);
  recentSalesOrders = signal<any[]>([]);

  async ngOnInit() {
    await this.loadDashboardData();
  }

  async loadDashboardData() {
    this.loading.set(true);
    try {
      const [stats, ordersData, purchaseOrdersData, salesOrdersData] = await Promise.all([
        this.dashboardService.getStats(),
        this.dashboardService.getRecentOrders(5),
        this.dashboardService.getRecentPurchaseOrders(5),
        this.dashboardService.getRecentSalesOrders(5)
      ]);

      this.statsData.set(stats);
      this.recentOrders.set(ordersData);
      this.recentPurchaseOrders.set(purchaseOrdersData);
      this.recentSalesOrders.set(salesOrdersData);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      this.loading.set(false);
    }
  }

  get statCards(): StatCard[] {
    const s = this.statsData();
    if (!s) return [];

    return [
      {
        title: this.i18n.t('dashboard.stats.products'),
        value: s.totalProducts,
        displayValue: this.formatCompactNumber(s.totalProducts),
        valueLabel: 'itens',
        route: '/products',
        color: 'text-blue-600 dark:text-blue-400',
        bgColor: 'bg-blue-100 dark:bg-blue-900/30',
        icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4'
      },
      {
        title: this.i18n.t('dashboard.stats.orders'),
        value: s.totalOrders,
        displayValue: this.formatCompactNumber(s.totalOrders),
        valueLabel: 'pedidos',
        secondaryValue: this.formatCompactCurrency(s.totalOrdersValue),
        secondaryLabel: 'valor total',
        trend: s.ordersTrend,
        route: '/orders',
        color: 'text-purple-600 dark:text-purple-400',
        bgColor: 'bg-purple-100 dark:bg-purple-900/30',
        icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01'
      },
      {
        title: this.i18n.t('dashboard.stats.customers'),
        value: s.totalCustomers,
        displayValue: this.formatCompactNumber(s.totalCustomers),
        valueLabel: 'clientes',
        secondaryValue: this.formatCompactCurrency(s.totalSalesOrdersValue),
        secondaryLabel: 'em vendas',
        trend: s.salesOrdersTrend,
        route: '/customers',
        color: 'text-emerald-600 dark:text-emerald-400',
        bgColor: 'bg-emerald-100 dark:bg-emerald-900/30',
        icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z'
      },
      {
        title: this.i18n.t('dashboard.stats.suppliers'),
        value: s.totalSuppliers,
        displayValue: this.formatCompactNumber(s.totalSuppliers),
        valueLabel: 'fornecedores',
        secondaryValue: this.formatCompactCurrency(s.totalPurchaseOrdersValue),
        secondaryLabel: 'em compras',
        trend: s.purchaseOrdersTrend,
        route: '/suppliers',
        color: 'text-orange-600 dark:text-orange-400',
        bgColor: 'bg-orange-100 dark:bg-orange-900/30',
        icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4'
      },
      {
        title: this.i18n.t('dashboard.stats.vehicles'),
        value: s.totalVehicles,
        displayValue: this.formatCompactNumber(s.totalVehicles),
        valueLabel: 'veículos',
        route: '/vehicles',
        color: 'text-cyan-600 dark:text-cyan-400',
        bgColor: 'bg-cyan-100 dark:bg-cyan-900/30',
        icon: 'M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4'
      },
      {
        title: this.i18n.t('dashboard.stats.drivers'),
        value: s.totalDrivers,
        displayValue: this.formatCompactNumber(s.totalDrivers),
        valueLabel: 'motoristas',
        route: '/drivers',
        color: 'text-indigo-600 dark:text-indigo-400',
        bgColor: 'bg-indigo-100 dark:bg-indigo-900/30',
        icon: 'M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z'
      },
      {
        title: this.i18n.t('dashboard.stats.inboundShipments'),
        value: s.pendingInboundShipments,
        displayValue: this.formatCompactNumber(s.pendingInboundShipments),
        valueLabel: 'pendentes',
        route: '/inbound-shipments',
        color: 'text-teal-600 dark:text-teal-400',
        bgColor: 'bg-teal-100 dark:bg-teal-900/30',
        icon: 'M3 4h13M3 8h9m-9 4h6m4 0l4-4m0 0l4 4m-4-4v12'
      },
      {
        title: this.i18n.t('dashboard.stats.outboundShipments'),
        value: s.pendingOutboundShipments,
        displayValue: this.formatCompactNumber(s.pendingOutboundShipments),
        valueLabel: 'pendentes',
        route: '/outbound-shipments',
        color: 'text-rose-600 dark:text-rose-400',
        bgColor: 'bg-rose-100 dark:bg-rose-900/30',
        icon: 'M3 4h13M3 8h9m-9 4h9m5-4v12m0 0l-4-4m4 4l4-4'
      }
    ];
  }

  get quickActions() {
    return [
      { label: this.i18n.t('dashboard.quickActions.newPurchaseOrder'), route: '/purchase-orders/new', icon: 'M12 6v6m0 0v6m0-6h6m-6 0H6', color: 'bg-blue-500 hover:bg-blue-600' },
      { label: this.i18n.t('dashboard.quickActions.newSalesOrder'), route: '/sales-orders/new', icon: 'M12 6v6m0 0v6m0-6h6m-6 0H6', color: 'bg-green-500 hover:bg-green-600' },
      { label: this.i18n.t('dashboard.quickActions.viewProducts'), route: '/products', icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4', color: 'bg-purple-500 hover:bg-purple-600' },
      { label: this.i18n.t('dashboard.quickActions.viewVehicles'), route: '/vehicles', icon: 'M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4', color: 'bg-cyan-500 hover:bg-cyan-600' }
    ];
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
  }

  navigateToOrder(order: any) {
    if (order.type === 'Purchase') {
      this.router.navigate(['/purchase-orders', order.id]);
    } else if (order.type === 'Sales') {
      this.router.navigate(['/sales-orders', order.id]);
    } else {
      this.router.navigate(['/orders', order.id]);
    }
  }

  getStatusColor(status: string | number): string {
    const statusLower = String(status ?? '').toLowerCase();
    if (statusLower.includes('completed') || statusLower.includes('concluído') || statusLower.includes('delivered')) {
      return 'text-green-600 bg-green-100 dark:text-green-400 dark:bg-green-900/30';
    }
    if (statusLower.includes('pending') || statusLower.includes('pendente') || statusLower.includes('processing')) {
      return 'text-amber-600 bg-amber-100 dark:text-amber-400 dark:bg-amber-900/30';
    }
    if (statusLower.includes('cancelled') || statusLower.includes('cancelado') || statusLower.includes('failed')) {
      return 'text-red-600 bg-red-100 dark:text-red-400 dark:bg-red-900/30';
    }
    return 'text-slate-600 bg-slate-100 dark:text-slate-400 dark:bg-slate-700';
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value || 0);
  }

  formatCompactNumber(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      notation: 'compact',
      compactDisplay: 'short',
      maximumFractionDigits: 1
    }).format(value || 0);
  }

  formatCompactCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
      notation: 'compact',
      compactDisplay: 'short',
      maximumFractionDigits: 1
    }).format(value || 0);
  }

  formatDate(date: string): string {
    if (!date) return 'N/A';
    return new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' }).format(new Date(date));
  }

  getInitials(name: string): string {
    if (!name) return '??';
    return name
      .split(' ')
      .map(n => n[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  getSparklinePoints(trend: number[]): string {
    if (!trend || trend.length === 0) return '';
    
    const max = Math.max(...trend, 1);
    const width = 100;
    const height = 30;
    const step = width / (trend.length - 1 || 1);
    
    return trend
      .map((val, i) => {
        const x = i * step;
        const y = height - (val / max) * (height - 4) - 2;
        return `${x},${y}`;
      })
      .join(' ');
  }
}
