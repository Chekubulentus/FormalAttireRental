import { Route, Routes } from "@angular/router";
import { CustomerLayoutComponent } from "./customer-layout/customer-layout.component";
import { Customer } from "../../data/models/DTOs/Customer/customer";

export const CUSTOMER_ROUTES: Routes = [
    {
        path: '',
        component: CustomerLayoutComponent,
        children: [
            {
                path: '',
                redirectTo: 'customer-dashboard',
                pathMatch: 'full'
            },
            {
                path: 'customer-dashboard',
                loadComponent: () => 
                    import('./customer-dashboard/customer-dashboard.component')
                .then(m => m.CustomerDashboardComponent)
            }
        ]
    }
]