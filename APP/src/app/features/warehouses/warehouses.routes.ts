import { Routes } from '@angular/router';

export const WAREHOUSES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./warehouses-list/warehouses-list.component')
      .then(m => m.WarehousesListComponent)
  }
];
