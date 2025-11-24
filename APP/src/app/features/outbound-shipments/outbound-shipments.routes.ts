import { Routes } from '@angular/router';

export const OUTBOUND_SHIPMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./outbound-shipments-list/outbound-shipments-list.component')
      .then(m => m.OutboundShipmentsListComponent)
  }
];
