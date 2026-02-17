import { CommonModule, NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-log-in',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './log-in.component.html',
  styleUrl: './log-in.component.scss',
})
export class LogInComponent {
  loginForm!: FormGroup;

  // UI State
  emailFocused = false;
  passwordFocused = false;
  showPassword = false;
  isLoading = false;
  loginError = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    // private authService: AuthService  // Uncomment when AuthService is ready
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false],
    });
  }

  /** Toggle password visibility */
  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  /** Handle login form submission */
  onLogin(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.loginError = '';

    const { email, password, rememberMe } = this.loginForm.value;

    // ── Replace this block with your real AuthService call ──────────────────
    // Example:
    // this.authService.login({ email, password, rememberMe }).subscribe({
    //   next: () => this.router.navigate(['/dashboard']),
    //   error: (err) => {
    //     this.loginError = err.message || 'Invalid credentials. Please try again.';
    //     this.isLoading  = false;
    //   }
    // });

    // ── Simulated login (remove once AuthService is wired up) ────────────────
    setTimeout(() => {
      this.isLoading = false;

      if (email === 'admin@elegance.com' && password === 'password') {
        this.router.navigate(['/dashboard']);
      } else {
        this.loginError = 'Invalid email or password. Please try again.';
      }
    }, 1800);
    // ─────────────────────────────────────────────────────────────────────────
  }

  /** Convenience getters for template validation */
  get emailCtrl() {
    return this.loginForm.get('email');
  }
  get passwordCtrl() {
    return this.loginForm.get('password');
  }
}
