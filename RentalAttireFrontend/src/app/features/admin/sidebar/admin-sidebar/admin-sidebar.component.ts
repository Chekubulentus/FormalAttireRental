import { Component } from '@angular/core';
import { AuthService } from '../../../../core/services/auth-service/auth.service';
import { Router, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLinkActive],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.scss',
})
export class AdminSidebarComponent {
  isCollapsed = false;

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  toggleCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/log-in']);
  }
}
