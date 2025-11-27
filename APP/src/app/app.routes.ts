import { Routes } from '@angular/router';
import { authGuard } from '@core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component')
      .then(m => m.LoginComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/main-layout/main-layout.component')
      .then(m => m.MainLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component')
          .then(m => m.DashboardComponent)
      },
      {
        path: 'products',
        loadComponent: () => import('./features/products/products-list/products-list.component')
          .then(m => m.ProductsListComponent)
      },
      {
        path: 'product-categories',
        loadComponent: () => import('./features/product-categories/product-categories.component')
          .then(m => m.ProductCategoriesComponent)
      },
      {
        path: 'purchase-orders',
        loadComponent: () => import('./features/purchase-orders/purchase-orders.component')
          .then(m => m.PurchaseOrdersComponent)
      },
      {
        path: 'customers',
        loadComponent: () => import('./features/customers/customers-list/customers-list.component')
          .then(m => m.CustomersListComponent)
      },
      {
        path: 'orders',
        loadComponent: () => import('./features/orders/orders-list/orders-list.component')
          .then(m => m.OrdersListComponent)
      },
      {
        path: 'warehouses',
        loadComponent: () => import('./features/warehouses/warehouses-list/warehouses-list.component')
          .then(m => m.WarehousesListComponent)
      },
      {
        path: 'inventory',
        loadComponent: () => import('./features/inventory/inventory-list/inventory-list.component')
          .then(m => m.InventoryListComponent)
      },
      {
        path: 'suppliers',
        loadComponent: () => import('./features/suppliers/suppliers-list/suppliers-list.component')
          .then(m => m.SuppliersListComponent)
      },
      {
        path: 'inbound-shipments',
        loadComponent: () => import('./features/inbound-shipments/inbound-shipments-list/inbound-shipments-list.component')
          .then(m => m.InboundShipmentsListComponent)
      },
      {
        path: 'outbound-shipments',
        loadComponent: () => import('./features/outbound-shipments/outbound-shipments-list/outbound-shipments-list.component')
          .then(m => m.OutboundShipmentsListComponent)
      },
      {
        path: 'picking-tasks',
        loadComponent: () => import('./features/picking-tasks/picking-tasks-list/picking-tasks-list.component')
          .then(m => m.PickingTasksListComponent)
      },
      {
        path: 'packing-tasks',
        loadComponent: () => import('./features/packing-tasks/packing-tasks-list/packing-tasks-list.component')
          .then(m => m.PackingTasksListComponent)
      },
      {
        path: 'vehicles',
        loadComponent: () => import('./features/vehicles/vehicles-list/vehicles-list.component')
          .then(m => m.VehiclesListComponent)
      },
      {
        path: 'drivers',
        loadComponent: () => import('./features/drivers/drivers-list/drivers-list.component')
          .then(m => m.DriversListComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
