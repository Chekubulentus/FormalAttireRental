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
      },
      {
        path: 'view-rental',
        loadComponent: () => {
          return import('./rentals/view-rental/view-rental.component')
          .then(m => m.ViewRentalComponent);
        }
      },
      {
        path: 'suppliers',
        loadComponent: () => {
          return import('./suppliers/suppliers/suppliers.component')
          .then(m => m.SuppliersComponent)
        }
      },
      {
        path: 'create-supplier',
        loadComponent: () => {
          return import('./suppliers/create-supplier-modal/create-supplier-modal.component')
          .then(m => m.CreateSupplierModalComponent)
        }
      }, 
      {
        path: 'view-supplier',
        loadComponent: () => {
          return import('./suppliers/view-supplier-modal/view-supplier-modal.component')
          .then(m => m.ViewSupplierModalComponent)
        }
      }
    ],
  },
];
