import { Routes } from '@angular/router';

export const PICKING_TASKS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./picking-tasks-list/picking-tasks-list.component')
      .then(m => m.PickingTasksListComponent)
  }
];
