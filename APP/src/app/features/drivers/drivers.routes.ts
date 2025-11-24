import { Routes } from '@angular/router';

export const DRIVERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./drivers-list/drivers-list.component')
      .then(m => m.DriversListComponent)
  }
];
