import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => {
            return import('./features/log-in/log-in/log-in.component').then(m => m.LogInComponent);
        }
    },
    {
        path: 'log-in',
        loadComponent: () => {
            return import('./features/log-in/log-in/log-in.component')
            .then(m => m.LogInComponent);
        }
    },
    {
        path: 'admin',
        loadComponent: () => {
            return import ('./features/admin/layout/admin-layout/admin-layout.component')
            .then(m => m.AdminLayoutComponent);
        },
        canActivate: [authGuard],
        children: [
            {
                path: 'admin-dashboard',
                loadComponent: () => {
                    return import('./features/admin/dashboard/admin-dashboard/admin-dashboard.component')
                    .then(m => m.AdminDashboardComponent);
                }
            },
            {
                path: '',
                redirectTo: 'admin-dashboard',
                pathMatch: 'full'
            }
        ]
    },
    {
        path: '',
        redirectTo: 'log-in',
        pathMatch: 'full'
    }
];
