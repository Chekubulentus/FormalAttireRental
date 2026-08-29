import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../core/services/auth-service/auth.service';
import { SocialAuthService } from '@abacritt/angularx-social-login';
import { CartService } from '../browse/cart.service';
import { CartComponent } from '../browse/cart/cart.component';
// import { UserService } from '../../../core/services/user-service/user.service';

@Component({
  selector: 'app-customer-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, CartComponent],
  templateUrl: './customer-layout.component.html',
  styleUrl: './customer-layout.component.scss',
})
export class CustomerLayoutComponent implements OnInit, OnDestroy {

  // ── Nav state ──────────────────────────────────────────────
  // NOTE: these are two SEPARATE flags on purpose — the desktop
  // user dropdown and the mobile hamburger drawer are different
  // pieces of UI and must not share one boolean, or opening one
  // silently opens the other's (invisible-on-desktop) full-screen
  // overlay elements too, which then fight the dropdown for clicks.
  menuOpen       = false; // desktop user-avatar dropdown
  mobileMenuOpen = false; // mobile hamburger drawer
  currentPath    = '';

  // ── Cart state ─────────────────────────────────────────────
  cartOpen  = false;
  cartCount = 0;
  private cartSub?: Subscription;

  // ── Current user ───────────────────────────────────────────
  customerName  = 'Guest';
  customerEmail = '';
  customerInitials = 'G';

  // Nav links
  navLinks = [
    { label: 'Dashboard',   path: '/customer/customer-dashboard', icon: 'dashboard' },
    { label: 'Browse',      path: '/customer/browse',             icon: 'browse'    },
    { label: 'My Rentals',  path: '/customer/my-rentals',         icon: 'rentals'   },
    { label: 'Profile',     path: '/customer/customer-profile',            icon: 'profile'   },
  ];

  constructor(
    private router: Router,
    // private userService: UserService,
    private socialAuthService: SocialAuthService,
    private authService: AuthService,
    private cartService: CartService,
  ) {}

  ngOnInit(): void {
    this.currentPath = this.router.url;

    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((e: any) => {
        this.currentPath    = e.urlAfterRedirects;
        this.menuOpen       = false;
        this.mobileMenuOpen = false;
      });

    this.cartSub = this.cartService.cartItems$.subscribe(items => {
      this.cartCount = items.reduce((sum, item) => sum + item.quantity, 0);
    });

    // TODO: load customer info
    // this.userService.getCurrentUserViewModel().then(res => {
    //   if (!res.isSuccess || !res.data) return;
    //   this.customerName     = res.data.fullName ?? 'Guest';
    //   this.customerEmail    = res.data.email ?? '';
    //   this.customerInitials = this.getInitials(this.customerName);
    // });
  }

  ngOnDestroy(): void {
    this.cartSub?.unsubscribe();
  }

  isActive(path: string): boolean {
    return this.currentPath.startsWith(path);
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  toggleCart(): void {
    this.cartOpen = !this.cartOpen;
  }

  getInitials(name: string): string {
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0][0]?.toUpperCase() ?? '?';
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }

  logout(): void {
    this.socialAuthService.signOut().catch(() => {});
    this.authService.logout();
  }
}