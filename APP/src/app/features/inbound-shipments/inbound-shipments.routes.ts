import { Routes } from '@angular/router';

export const INBOUND_SHIPMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./inbound-shipments-list/inbound-shipments-list.component')
      .then(m => m.InboundShipmentsListComponent)
  }
];
