import { Routes } from '@angular/router';

export const PACKING_TASKS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./packing-tasks-list/packing-tasks-list.component')
      .then(m => m.PackingTasksListComponent)
  }
];
