import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
// import { ProfileService } from '../../../core/services/profile-service/profile.service';
// import { AppToastrService } from '../../../core/services/toastr-service/app-toastr.service';
// import { AuthService } from '../../../core/services/auth-service/auth.service';

// ── Inline command — replace with real import once created ───
export class CompleteProfileCommand {
  // Personal
  firstName: string     = '';
  lastName: string      = '';
  middleName: string    = '';
  age: number           = 0;
  gender: string        = '';
  maritalStatus: string = '';
  phoneNumber: string   = '';

  // Address
  street: string     = '';
  barangay: string   = '';
  city: string       = '';
  province: string   = '';
  postalCode: string = '';
}
// ─────────────────────────────────────────────────────────────

@Component({
  selector: 'app-profile-completion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile-completion.component.html',
  styleUrl: './profile-completion.component.scss',
})
export class ProfileCompletionComponent implements OnInit {

  form: CompleteProfileCommand = new CompleteProfileCommand();

  // ── State ──────────────────────────────────────────────────
  isSubmitting = false;
  touched      = false;

  // ── Step tracking (2 steps) ────────────────────────────────
  currentStep = 1;
  totalSteps  = 2;

  // ── Dropdowns ─────────────────────────────────────────────
  genders        = ['Male', 'Female'];
  maritalStatuses = ['Single', 'Married', 'Widowed', 'Separated'];

  // Pre-filled from Google account (read-only display)
  googleName  = '';
  googleEmail = '';

  constructor(
    private router: Router,
    // private profileService: ProfileService,
    // private toastr: AppToastrService,
    // private authService: AuthService,
  ) {}

  ngOnInit(): void {
    // TODO: pre-fill name from stored Google user data
    // const storedUser = localStorage.getItem(CurrentUser);
    // if (storedUser) {
    //   const user = JSON.parse(storedUser);
    //   this.googleName  = user.fullName ?? '';
    //   this.googleEmail = user.email ?? '';
    //   // Pre-fill first/last from Google if available
    //   this.form.firstName = user.firstName ?? '';
    //   this.form.lastName  = user.lastName  ?? '';
    // }
  }


  // ============================================================
  // Validation
  // ============================================================
  get step1Errors(): Record<string, string> {
    const f = this.form;
    const e: Record<string, string> = {};

    if (!f.lastName?.trim())       e['lastName']      = 'Last name is required.';
    if (!f.firstName?.trim())      e['firstName']     = 'First name is required.';
    if (!f.age || f.age < 1)       e['age']           = 'Enter a valid age.';
    if (f.age > 120)               e['age']           = 'Age cannot exceed 120.';
    if (!f.gender)                 e['gender']        = 'Select a gender.';
    if (!f.maritalStatus)          e['maritalStatus'] = 'Select marital status.';
    if (!f.phoneNumber?.trim())
      e['phoneNumber'] = 'Phone number is required.';
    else if (!/^09\d{9}$/.test(f.phoneNumber.trim()))
      e['phoneNumber'] = 'Must be 11 digits starting with 09.';

    return e;
  }

  get step2Errors(): Record<string, string> {
    const f = this.form;
    const e: Record<string, string> = {};

    if (!f.street?.trim())     e['street']     = 'Street is required.';
    if (!f.barangay?.trim())   e['barangay']   = 'Barangay is required.';
    if (!f.city?.trim())       e['city']       = 'City is required.';
    if (!f.province?.trim())   e['province']   = 'Province is required.';
    if (!f.postalCode?.trim()) e['postalCode'] = 'Postal code is required.';

    return e;
  }

  get currentErrors(): Record<string, string> {
    return this.currentStep === 1 ? this.step1Errors : this.step2Errors;
  }

  get isCurrentStepValid(): boolean {
    return Object.keys(this.currentErrors).length === 0;
  }

  hasError(field: string): boolean {
    return this.touched && !!this.currentErrors[field];
  }

  errorMsg(field: string): string {
    return this.touched ? (this.currentErrors[field] ?? '') : '';
  }


  // ============================================================
  // Step Navigation
  // ============================================================
  nextStep(): void {
    this.touched = true;
    if (!this.isCurrentStepValid) return;
    this.touched     = false;
    this.currentStep = 2;
  }

  prevStep(): void {
    this.touched     = false;
    this.currentStep = 1;
  }


  // ============================================================
  // Submit
  // ============================================================
  submit(): void {
    this.touched = true;
    if (!this.isCurrentStepValid) return;

    this.isSubmitting = true;

    // TODO: wire to ProfileService
    // this.profileService.completeProfileAsync(this.form)
    //   .then(res => {
    //     if (!res.isSuccess) {
    //       this.toastr.error(res.errorMessage ?? 'Failed to complete profile.');
    //       return;
    //     }
    //     this.toastr.success('Profile completed successfully!');
    //     this.router.navigateByUrl('/customer/customer-dashboard');
    //   })
    //   .catch(err => console.error(err))
    //   .finally(() => this.isSubmitting = false);

    console.log('Complete profile payload:', this.form);
    this.isSubmitting = false;
    this.router.navigateByUrl('/customer/customer-dashboard');
  }
}