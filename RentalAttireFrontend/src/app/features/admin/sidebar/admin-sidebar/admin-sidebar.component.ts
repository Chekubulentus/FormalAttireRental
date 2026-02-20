import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../../core/services/auth-service/auth.service';
import { Router, RouterLinkActive, UrlSegment } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserViewModel } from '../../../../data/models/DTOs/Users/user-view-model';
import { UserService } from '../../../../core/services/user-service/user.service';
import { CurrentUser } from '../../../../../environments/current-user';
import { Result } from '../../../../data/models/Results/result';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLinkActive, FormsModule],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.scss',
})
export class AdminSidebarComponent implements OnInit{
  isCollapsed = false;
  currentUser : UserViewModel | undefined;

  constructor(
    private authService: AuthService,
    private router: Router,
    private userService : UserService
  ) {}

  ngOnInit(): void {
    this.getCurrentUser();
  }

  getCurrentUser(){
    const user = this.authService.getCurrentUser();
    if(!user)
      return;
    const parsedUser = JSON.parse(user);
    const result = this.userService.getUserViewModelByIdAsync(parsedUser.id)
    .then(
      res => {
        if(!res.isSuccess){
          console.log("pukign;kalsjdqlwke");
        }
        console.log(res);
        this.currentUser = res.data;
        console.log(this.currentUser);
      }
    ).catch(
      err => {
        console.log(`${err.error}`);
      }
    )
  }

  toggleCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/log-in']);
  }
}
