import { Route, Routes } from "@angular/router";
import { CustomerLayoutComponent } from "./customer-layout/customer-layout.component";

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
            },
            {
                path: 'browse',
                loadComponent: () => {
                    return import('./browse/browse/browse.component')
                    .then(m => m.BrowseComponent)
                }
            },
            {
                path: 'view-clothe',
                loadComponent: () => {
                    return import('./browse/view-clothe-modal/view-clothe-modal.component')
                    .then(m => m.ViewClotheModalComponent)
                }
            },
            {
                path: 'cart',
                loadComponent() {
                    return import('./browse/cart/cart.component')
                    .then(m => m.CartComponent)
                },
            },
            {
                path: 'reservation',
                loadComponent: () => {
                    return import ('./browse/reservation/reservation.component')
                    .then(m => m.ReservationComponent)
                }
            },
            {
                path: 'my-rentals',
                loadComponent: () => {
                    return import('./my-rental/my-rental/my-rental.component')
                    .then(m => m.MyRentalsComponent)
                }
            },
            {
                path: 'customer-profile',
                loadComponent: () => {
                    return import('./my-profile/customer-profile/customer-profile.component')
                    .then(m => m.CustomerProfileComponent);
                }
            }
        ]
    }
]