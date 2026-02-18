import { CommonModule, NgIf } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service/auth.service';

@Component({
  selector: 'app-log-in',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './log-in.component.html',
  styleUrl: './log-in.component.scss',
})
export class LogInComponent {
  loginForm!: FormGroup;
  email: string = '';
  password: string = '';

  // UI State
  emailFocused = false;
  passwordFocused = false;
  showPassword = false;
  isLoading = false;
  loginError : string | undefined;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      rememberMe: [false],
    });
  }


  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }


  onLogin(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.loginError = '';

    const { email, password, rememberMe } = this.loginForm.value;

    const result = this.authService
      .login(email, password)
      .then((result) => {
        if (!result.isSuccess) {
          this.loginError = result.errorMessage;
        }
        console.log(`Logged in successfully.`);
      })
      .catch((err) => {
        console.log(`${err.error?.errorMessage}`);
      })
      .finally(() => {
        this.isLoading = false;
      });
  }

  get emailCtrl() {
    return this.loginForm.get('email');
  }
  get passwordCtrl() {
    return this.loginForm.get('password');
  }
}
