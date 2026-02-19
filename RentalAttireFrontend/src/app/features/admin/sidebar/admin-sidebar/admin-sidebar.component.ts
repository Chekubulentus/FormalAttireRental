import { Component } from '@angular/core';
import { AuthService } from '../../../../core/services/auth-service/auth.service';
import { Router, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserDTO } from '../../../../data/models/user-dto';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLinkActive, FormsModule],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.scss',
})
export class AdminSidebarComponent {
  isCollapsed = false;
  currentUser : UserDTO = new UserDTO();

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
