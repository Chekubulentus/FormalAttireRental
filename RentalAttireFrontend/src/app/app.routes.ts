import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => {
            return import('./features/landing-page/landing-page/landing-page.component')
            .then(
                m => m.LandingPageComponent
            );
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
        path: 'register',
        loadComponent: () => {
            return import('./features/log-in/registration/registration.component')
            .then(m => m.RegistrationComponent);
        }
    },
    {
        path: 'admin',
        canActivate: [authGuard],
        loadChildren: () => 
            import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES)
    },
    {
        path: 'customer',
        canActivate: [authGuard],
        loadChildren: () => 
            import('./features/customer/customer.routes').then(m => m.CUSTOMER_ROUTES)
    },
    {
        path: 'profile-completion',
        loadComponent: () => {
            return import('./features/log-in/profile-completion/profile-completion.component')
            .then(m => m.ProfileCompletionComponent)
        }
    },
    {
      path: 'landing-page',
      loadComponent: () => {
        return import('./features/landing-page/landing-page/landing-page.component')
        .then(
            m => m.LandingPageComponent
        );
      }  
    },
    {
        path: '',
        redirectTo: 'landing-page',
        pathMatch: 'full'
    }
];
