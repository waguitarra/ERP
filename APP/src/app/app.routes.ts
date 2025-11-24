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
        loadChildren: () => import('./features/products/products.routes')
          .then(m => m.PRODUCTS_ROUTES)
      },
      {
        path: 'customers',
        loadChildren: () => import('./features/customers/customers.routes')
          .then(m => m.CUSTOMERS_ROUTES)
      },
      {
        path: 'orders',
        loadChildren: () => import('./features/orders/orders.routes')
          .then(m => m.ORDERS_ROUTES)
      },
      {
        path: 'warehouses',
        loadChildren: () => import('./features/warehouses/warehouses.routes')
          .then(m => m.WAREHOUSES_ROUTES)
      },
      {
        path: 'inventory',
        loadChildren: () => import('./features/inventory/inventory.routes')
          .then(m => m.INVENTORY_ROUTES)
      },
      {
        path: 'suppliers',
        loadChildren: () => import('./features/suppliers/suppliers.routes')
          .then(m => m.SUPPLIERS_ROUTES)
      },
      {
        path: 'inbound-shipments',
        loadChildren: () => import('./features/inbound-shipments/inbound-shipments.routes')
          .then(m => m.INBOUND_SHIPMENTS_ROUTES)
      },
      {
        path: 'outbound-shipments',
        loadChildren: () => import('./features/outbound-shipments/outbound-shipments.routes')
          .then(m => m.OUTBOUND_SHIPMENTS_ROUTES)
      },
      {
        path: 'picking-tasks',
        loadChildren: () => import('./features/picking-tasks/picking-tasks.routes')
          .then(m => m.PICKING_TASKS_ROUTES)
      },
      {
        path: 'packing-tasks',
        loadChildren: () => import('./features/packing-tasks/packing-tasks.routes')
          .then(m => m.PACKING_TASKS_ROUTES)
      },
      {
        path: 'vehicles',
        loadChildren: () => import('./features/vehicles/vehicles.routes')
          .then(m => m.VEHICLES_ROUTES)
      },
      {
        path: 'drivers',
        loadChildren: () => import('./features/drivers/drivers.routes')
          .then(m => m.DRIVERS_ROUTES)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
