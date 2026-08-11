import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../../core/services/auth-service/auth.service';
import { SocialAuthService } from '@abacritt/angularx-social-login';
// import { UserService } from '../../../core/services/user-service/user.service';
// import { AuthService } from '../../../core/services/auth-service/auth.service';
// import { SocialAuthService } from '@abacritt/angularx-social-login';

@Component({
  selector: 'app-customer-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './customer-layout.component.html',
  styleUrl: './customer-layout.component.scss',
})
export class CustomerLayoutComponent implements OnInit {

  // ── Nav state ──────────────────────────────────────────────
  menuOpen    = false;
  currentPath = '';

  // ── Current user ───────────────────────────────────────────
  customerName  = 'Guest';
  customerEmail = '';
  customerInitials = 'G';

  // Nav links
  navLinks = [
    { label: 'Dashboard',   path: '/customer/customer-dashboard', icon: 'dashboard' },
    { label: 'Browse',      path: '/customer/browse',             icon: 'browse'    },
    { label: 'My Rentals',  path: '/customer/my-rentals',         icon: 'rentals'   },
    { label: 'Profile',     path: '/customer/profile',            icon: 'profile'   },
  ];

  constructor(
    private router: Router,
    // private userService: UserService,
    // private authService: AuthService,
    // private socialAuthService: SocialAuthService,
    private socialAuthService: SocialAuthService,
    private authService : AuthService
  ) {}

  ngOnInit(): void {
    this.currentPath = this.router.url;

    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((e: any) => {
        this.currentPath = e.urlAfterRedirects;
        this.menuOpen    = false;
      });

    // TODO: load customer info
    // this.userService.getCurrentUserViewModel().then(res => {
    //   if (!res.isSuccess || !res.data) return;
    //   this.customerName     = res.data.fullName ?? 'Guest';
    //   this.customerEmail    = res.data.email ?? '';
    //   this.customerInitials = this.getInitials(this.customerName);
    // });
  }

  isActive(path: string): boolean {
    return this.currentPath.startsWith(path);
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  getInitials(name: string): string {
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0][0]?.toUpperCase() ?? '?';
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }

  logout(): void {
    // TODO: wire logout
    // this.socialAuthService.signOut().catch(() => {});
    // this.authService.logout();
    this.socialAuthService.signOut().catch(() => {});
    this.authService.logout();
  }
}