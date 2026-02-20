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
        canActivate: [authGuard],
        loadChildren: () => 
            import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES)
    },
    {
        path: '',
        redirectTo: 'log-in',
        pathMatch: 'full'
    }
];
