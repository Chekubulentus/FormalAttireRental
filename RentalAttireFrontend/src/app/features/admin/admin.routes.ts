import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './layout/admin-layout/admin-layout.component';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', redirectTo: 'admin-dashboard', pathMatch: 'full' },
      {
        path: 'admin-dashboard',
        loadComponent: () =>
          import('./dashboard/admin-dashboard/admin-dashboard.component')
          .then(m => m.AdminDashboardComponent)
      },
      {
        path: 'user-layout',
        loadChildren: () =>
          import('./users/user.routes')
          .then(m => m.USER_ROUTES)
      }
    ]
  }
];