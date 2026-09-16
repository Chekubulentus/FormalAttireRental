import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../../../core/services/user-service/user.service';
import { AuthService } from '../../../../core/services/auth-service/auth.service';
import { UserViewModel } from '../../../../data/models/DTOs/Users/user-view-model';
import { ToastrService } from 'ngx-toastr';
import { SocialAuthService } from '@abacritt/angularx-social-login';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.scss',
})
export class AdminSidebarComponent implements OnInit {

  // ============================================================
  // State
  // ============================================================

  // Whether the sidebar is collapsed to icon-only mode
  isCollapsed = false;

  // Tracks which nav groups are open — all open by default
  groups: Record<string, boolean> = {
    overview:   true,
    management: true,
    system:     true,
  };
  
  currentUser: UserViewModel | undefined; 

  constructor(
    private router: Router,
    private userService: UserService,
    private authService : AuthService,
    private toastrService : ToastrService,
    private socialAuthService : SocialAuthService
  ) {}

  ngOnInit(): void {
    this.getCurrentUser();
  }

  // ============================================================
  // Collapse / Expand sidebar
  // ============================================================
  toggleCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
  }

  // ============================================================
  // Toggle a nav group open or closed
  // When the sidebar is collapsed all groups are always visible
  // (controlled in the template via [class.expanded]="groups[x] || isCollapsed")
  // ============================================================
  toggleGroup(group: string): void {
    this.groups[group] = !this.groups[group];
  }

  // ============================================================
  // Load current user for the footer
  // ============================================================
  getCurrentUser(): void {
    this.userService.getCurrentUserViewModel()
    .then(res => {
      if(!res.isSuccess)
        this.toastrService.error(res.errorMessage ?? 'Current user cannot be found.');
      this.currentUser = res.data;
    }).catch(err => {
      this.toastrService.error(err.error);
    })
  }

  // ============================================================
  // Logout
  // ============================================================
  logout(): void {
    this.socialAuthService.signOut().catch(() => {});
    this.authService.logout();
  }
}