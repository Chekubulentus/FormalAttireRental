import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => {
            return import('./features/log-in/log-in/log-in.component').then(m => m.LogInComponent);
        }
    }
];
