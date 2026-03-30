import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth-service/auth.service';
import {
  SocialAuthService,
  SocialUser,
  SocialLoginModule,
} from '@abacritt/angularx-social-login';
import { Subscription } from 'rxjs';
import { CurrentUser } from '../../../../environments/current-user';

@Component({
  selector: 'app-log-in',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SocialLoginModule],
  templateUrl: './log-in.component.html',
  styleUrl: './log-in.component.scss',
})
export class LogInComponent implements OnInit, OnDestroy {

  loginForm!: FormGroup;

  // UI State
  emailFocused    = false;
  passwordFocused = false;
  showPassword    = false;
  isLoading       = false;
  isGoogleLoading = false;
  loginError: string | undefined;

  private authStateSub!: Subscription;
  private isLoggingOut = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private socialAuthService: SocialAuthService,
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email:      ['', [Validators.required, Validators.email]],
      password:   ['', [Validators.required]],
      rememberMe: [false],
    });

    // Fires when the Google button is clicked and the user completes sign-in
    this.authStateSub = this.socialAuthService.authState.subscribe((user: SocialUser) => {
      if (!user?.idToken || this.isLoggingOut) return;

      this.isGoogleLoading = true;
      this.loginError      = undefined;

      this.authService
        .googleLogin(user.idToken)
        .then((result) => {
          if (!result.isSuccess) {
            this.loginError = result.errorMessage;
            return;
          }
          if (result.data?.accessToken && result.data?.refreshToken) {
            this.authService.saveTokens(result.data.accessToken, result.data.refreshToken);
            localStorage.setItem(CurrentUser, JSON.stringify(result.data.user));
          }
          this.router.navigateByUrl('/admin');
        })
        .catch((err) => {
          this.loginError = err?.error?.errorMessage ?? 'Google sign-in failed.';
          console.error('Google login error:', err);
        })
        .finally(() => {
          this.isGoogleLoading = false;
        });
    });
  }

  ngOnDestroy(): void {
    this.authStateSub?.unsubscribe();
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  // ── Standard login ─────────────────────────────────────────
  onLogin(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading  = true;
    this.loginError = undefined;

    const { email, password } = this.loginForm.value;

    this.authService
      .login(email, password)
      .then((result) => {
        if (!result.isSuccess) {
          this.loginError = result.errorMessage;
          return;
        }
        if (result.data?.accessToken && result.data?.refreshToken) {
          this.authService.saveTokens(result.data.accessToken, result.data.refreshToken);
        }
        this.router.navigateByUrl('/admin');
      })
      .catch((err) => {
        this.loginError = err?.error?.errorMessage ?? 'An unexpected error occurred.';
        console.error('Login error:', err);
      })
      .finally(() => {
        this.isLoading = false;
      });
  }

  get emailCtrl()    { return this.loginForm.get('email'); }
  get passwordCtrl() { return this.loginForm.get('password'); }
}