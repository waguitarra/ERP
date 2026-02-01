import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { AuthService } from './auth.service';

export interface DashboardStats {
  totalProducts: number;
  totalOrders: number;
  totalCustomers: number;
  totalSuppliers: number;
  totalVehicles: number;
  totalDrivers: number;
  totalWarehouses: number;
  pendingInboundShipments: number;
  pendingOutboundShipments: number;
  totalInventoryItems: number;
  totalStockMovements: number;
  totalPickingTasks: number;
  totalPackingTasks: number;
  totalPurchaseOrders: number;
  totalSalesOrders: number;
  // Valores monetários
  totalPurchaseOrdersValue: number;
  totalSalesOrdersValue: number;
  totalOrdersValue: number;
  // Dados para gráficos (últimos 7 dias simulados)
  purchaseOrdersTrend: number[];
  salesOrdersTrend: number[];
  ordersTrend: number[];
}

export interface RecentOrder {
  id: string;
  orderNumber: string;
  customerName: string;
  supplierName: string;
  type: string;
  totalValue: number;
  status: string;
  orderDate: string;
}

export interface RecentShipment {
  id: string;
  shipmentNumber: string;
  type: 'inbound' | 'outbound';
  status: string;
  expectedDate: string;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly api = inject(ApiService);
  private readonly authService = inject(AuthService);

  private get companyId(): string {
    return this.authService.currentUser()?.companyId || 'a1b2c3d4-5555-1111-2222-333333333333';
  }

  async getStats(): Promise<DashboardStats> {
    try {
      const companyId = this.companyId;
      
      // Buscar contagens de cada entidade em paralelo
      const [
        products,
        customers,
        suppliers,
        vehicles,
        drivers,
        warehouses,
        inboundShipments,
        outboundShipments,
        purchaseOrders,
        salesOrders,
        orders
      ] = await Promise.all([
        this.api.get<any>('/products').catch(() => []),
        this.api.get<any>('/customers').catch(() => []),
        this.api.get<any>('/suppliers').catch(() => []),
        this.api.get<any>('/vehicles').catch(() => []),
        this.api.get<any>('/drivers').catch(() => []),
        this.api.get<any>('/warehouses').catch(() => []),
        this.api.get<any>('/inbound-shipments').catch(() => []),
        this.api.get<any>('/outbound-shipments').catch(() => []),
        this.api.get<any>(`/purchase-orders/company/${companyId}`).catch(() => ({ data: [] })),
        this.api.get<any>(`/sales-orders/company/${companyId}`).catch(() => ({ data: [] })),
        this.api.get<any>(`/orders/company/${companyId}`).catch(() => [])
      ]);

      const poItems = this.extractItems(purchaseOrders);
      const soItems = this.extractItems(salesOrders);
      const orderItems = this.extractItems(orders);
      
      const totalPOValue = poItems.reduce((sum: number, po: any) => sum + (po.totalValue || 0), 0);
      const totalSOValue = soItems.reduce((sum: number, so: any) => sum + (so.totalValue || 0), 0);
      const totalOrdersVal = orderItems.reduce((sum: number, o: any) => sum + (o.totalValue || 0), 0);

      // Contagem de shipments pendentes (não completados)
      const inboundItems = this.extractItems(inboundShipments);
      const outboundItems = this.extractItems(outboundShipments);
      const pendingInbound = inboundItems.filter((s: any) => 
        s.status !== 'Completed' && s.status !== 'Received' && s.statusName !== 'Completed'
      ).length;
      const pendingOutbound = outboundItems.filter((s: any) => 
        s.status !== 2 && s.status !== 'Delivered' && s.status !== 'Completed'
      ).length;

      return {
        totalProducts: this.extractCount(products),
        totalOrders: this.extractCount(orders),
        totalCustomers: this.extractCount(customers),
        totalSuppliers: this.extractCount(suppliers),
        totalVehicles: this.extractCount(vehicles),
        totalDrivers: this.extractCount(drivers),
        totalWarehouses: this.extractCount(warehouses),
        pendingInboundShipments: pendingInbound,
        pendingOutboundShipments: pendingOutbound,
        totalInventoryItems: 0,
        totalStockMovements: 0,
        totalPickingTasks: 0,
        totalPackingTasks: 0,
        totalPurchaseOrders: this.extractCount(purchaseOrders),
        totalSalesOrders: this.extractCount(salesOrders),
        totalPurchaseOrdersValue: totalPOValue,
        totalSalesOrdersValue: totalSOValue,
        totalOrdersValue: totalOrdersVal,
        purchaseOrdersTrend: this.generateTrendData(poItems),
        salesOrdersTrend: this.generateTrendData(soItems),
        ordersTrend: this.generateTrendData(orderItems)
      };
    } catch (error) {
      console.error('Error fetching dashboard stats:', error);
      return this.getEmptyStats();
    }
  }

  async getRecentOrders(limit: number = 5): Promise<RecentOrder[]> {
    try {
      const companyId = this.companyId;
      
      // Combinar purchase orders e sales orders como "orders"
      const [purchaseOrders, salesOrders] = await Promise.all([
        this.api.get<any>(`/purchase-orders/company/${companyId}`).catch(() => ({ data: [] })),
        this.api.get<any>(`/sales-orders/company/${companyId}`).catch(() => ({ data: [] }))
      ]);

      const poItems = this.extractItems(purchaseOrders).map((po: any) => ({
        id: po.id,
        orderNumber: po.purchaseOrderNumber || `PO-${po.id?.substring(0, 8)}`,
        customerName: '',
        supplierName: po.supplierName || 'N/A',
        type: 'Purchase',
        totalValue: po.totalValue || 0,
        status: po.status || 'Pending',
        orderDate: po.createdAt || new Date().toISOString()
      }));

      const soItems = this.extractItems(salesOrders).map((so: any) => ({
        id: so.id,
        orderNumber: so.salesOrderNumber || `SO-${so.id?.substring(0, 8)}`,
        customerName: so.customerName || 'N/A',
        supplierName: '',
        type: 'Sales',
        totalValue: so.totalValue || 0,
        status: so.status || 'Pending',
        orderDate: so.createdAt || new Date().toISOString()
      }));

      // Combinar e ordenar por data
      return [...poItems, ...soItems]
        .sort((a, b) => new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime())
        .slice(0, limit);
    } catch (error) {
      console.error('Error fetching recent orders:', error);
      return [];
    }
  }

  async getRecentPurchaseOrders(limit: number = 5): Promise<any[]> {
    try {
      const companyId = this.companyId;
      const response = await this.api.get<any>(`/purchase-orders/company/${companyId}`);
      return this.extractItems(response).slice(0, limit);
    } catch (error) {
      console.error('Error fetching recent purchase orders:', error);
      return [];
    }
  }

  async getRecentSalesOrders(limit: number = 5): Promise<any[]> {
    try {
      const companyId = this.companyId;
      const response = await this.api.get<any>(`/sales-orders/company/${companyId}`);
      return this.extractItems(response).slice(0, limit);
    } catch (error) {
      console.error('Error fetching recent sales orders:', error);
      return [];
    }
  }

  async getRecentShipments(limit: number = 5): Promise<RecentShipment[]> {
    try {
      const [inbound, outbound] = await Promise.all([
        this.api.get<any>('/inbound-shipments', { pageSize: limit }).catch(() => ({ items: [] })),
        this.api.get<any>('/outbound-shipments', { pageSize: limit }).catch(() => ({ items: [] }))
      ]);

      const inboundItems = (inbound?.items || inbound || []).map((s: any) => ({
        id: s.id,
        shipmentNumber: s.shipmentNumber,
        type: 'inbound' as const,
        status: s.status,
        expectedDate: s.expectedArrivalDate
      }));

      const outboundItems = (outbound?.items || outbound || []).map((s: any) => ({
        id: s.id,
        shipmentNumber: s.shipmentNumber,
        type: 'outbound' as const,
        status: s.status,
        expectedDate: s.shippedDate
      }));

      return [...inboundItems, ...outboundItems].slice(0, limit);
    } catch (error) {
      console.error('Error fetching recent shipments:', error);
      return [];
    }
  }

  private extractCount(response: any): number {
    if (typeof response === 'number') return response;
    if (response?.totalCount !== undefined) return response.totalCount;
    if (response?.total !== undefined) return response.total;
    if (response?.count !== undefined) return response.count;
    if (response?.data && Array.isArray(response.data)) return response.data.length;
    if (Array.isArray(response)) return response.length;
    if (response?.items && Array.isArray(response.items)) return response.items.length;
    return 0;
  }

  private extractItems(response: any): any[] {
    if (Array.isArray(response)) return response;
    if (response?.data && Array.isArray(response.data)) return response.data;
    if (response?.items && Array.isArray(response.items)) return response.items;
    return [];
  }

  private generateTrendData(items: any[]): number[] {
    const trend: number[] = [];
    const now = new Date();
    for (let i = 6; i >= 0; i--) {
      const date = new Date(now);
      date.setDate(date.getDate() - i);
      const dayStart = new Date(date.setHours(0, 0, 0, 0));
      const dayEnd = new Date(date.setHours(23, 59, 59, 999));
      
      const dayTotal = items
        .filter((item: any) => {
          const itemDate = new Date(item.orderDate || item.createdAt);
          return itemDate >= dayStart && itemDate <= dayEnd;
        })
        .reduce((sum: number, item: any) => sum + (item.totalValue || 0), 0);
      
      trend.push(dayTotal);
    }
    return trend;
  }

  private generateCombinedTrend(poItems: any[], soItems: any[]): number[] {
    const poTrend = this.generateTrendData(poItems);
    const soTrend = this.generateTrendData(soItems);
    return poTrend.map((val, i) => val + soTrend[i]);
  }

  private getEmptyStats(): DashboardStats {
    return {
      totalProducts: 0,
      totalOrders: 0,
      totalCustomers: 0,
      totalSuppliers: 0,
      totalVehicles: 0,
      totalDrivers: 0,
      totalWarehouses: 0,
      pendingInboundShipments: 0,
      pendingOutboundShipments: 0,
      totalInventoryItems: 0,
      totalStockMovements: 0,
      totalPickingTasks: 0,
      totalPackingTasks: 0,
      totalPurchaseOrders: 0,
      totalSalesOrders: 0,
      totalPurchaseOrdersValue: 0,
      totalSalesOrdersValue: 0,
      totalOrdersValue: 0,
      purchaseOrdersTrend: [0, 0, 0, 0, 0, 0, 0],
      salesOrdersTrend: [0, 0, 0, 0, 0, 0, 0],
      ordersTrend: [0, 0, 0, 0, 0, 0, 0]
    };
  }
}
