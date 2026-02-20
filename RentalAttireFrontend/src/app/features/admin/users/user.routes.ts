import { Route, Routes } from "@angular/router";
import { UserLayoutComponent } from "./user-layout/user-layout.component";

export const USER_ROUTES: Routes = [
    {
        path: '',
        component: UserLayoutComponent,
        children: [
            {
                path: '',
                redirectTo: 'employees',
                pathMatch: 'full'
            },
            {
                path: 'employees',
                loadComponent: () => {
                    return import('./employees/employees.component')
                    .then(m => m.EmployeesComponent);
                }
            },
            {
                path: 'accounts',
                loadComponent: () => {
                    return import('./accounts/accounts.component')
                    .then(m => m.AccountsComponent);
                }
            }
        ]
    }
];