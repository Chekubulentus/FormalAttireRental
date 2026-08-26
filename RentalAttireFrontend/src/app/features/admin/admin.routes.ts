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
          import('./dashboard/admin-dashboard/admin-dashboard.component').then(
            (m) => m.AdminDashboardComponent,
          ),
      },
      {
        path: 'user-layout',
        loadChildren: () =>
          import('./users/user.routes').then((m) => m.USER_ROUTES),
      },
      {
        path: 'logs',
        loadComponent: () => {
          return import('./logs/logs/logs.component').then(
            (m) => m.LogsComponent,
          );
        },
      },
      {
        path: 'clothes',
        loadComponent: () => {
          return import('./clothes/clothes/clothes.component').then(
            (m) => m.ClothesComponent,
          );
        },
      },
      {
        path: 'category',
        loadComponent: () => {
          return import('./categories/category/category.component').then(
            (m) => m.CategoryComponent,
          );
        },
      },
      {
        path: 'customer',
        loadComponent: () => {
          return import('./customers/customer/customer.component').then(
            (m) => m.CustomerComponent,
          );
        },
      },
      {
        path: 'disposables',
        loadComponent: () => {
          return import('./disposables/disposables/disposables.component')
          .then(m => m.DisposablesComponent)
        }
      },
      {
        path: 'rentals',
        loadComponent: () => {
          return import('./rentals/rentals/rentals.component')
          .then(m => m.RentalsComponent);
        }
      }
    ],
  },
];
