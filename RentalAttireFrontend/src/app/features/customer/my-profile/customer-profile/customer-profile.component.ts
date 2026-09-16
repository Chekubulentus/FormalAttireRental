// src/app/features/customer/profile/customer-profile.component.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CustomerProfileService } from '../customer-profile.service';
import { Customer } from '../../../../data/models/DTOs/Customer/customer';

@Component({
  selector: 'app-customer-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer-profile.component.html',
  styleUrl: './customer-profile.component.scss',
})
export class CustomerProfileComponent implements OnInit {
  loading = true;
  saving = false;
  profile: Customer | null = null;

  // ── Form model (Person + password fields) ──────────────────
  form = {
    lastName: '',
    firstName: '',
    middleName: '',
    age: 0,
    gender: 'Male',
    maritalStatus: 'Single',
    phoneNumber: '',
    street: '',
    barangay: '',
    city: '',
    province: '',
    postalCode: '',
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  };

  genders = ['Male', 'Female', 'Others'];
  maritalStatuses = [
    'Single',
    'Married',
    'Seperated',
    'Divorced',
    'Widowed',
    'Annulled',
  ];

  selectedImage: File | null = null;
  imagePreviewUrl: string | null = null;

  touched = false;

  constructor(
    private profileService: CustomerProfileService,
    private toastr: ToastrService,
  ) {}

  async ngOnInit(): Promise<void> {
    const res = await this.profileService.getCustomerProfileAsync();
    this.loading = false;

    if (!res.isSuccess || !res.data) {
      this.toastr.error(res.errorMessage || 'Could not load profile.');
      return;
    }

    this.profile = res.data;
    const p = res.data.person;

    this.form = {
      ...this.form,
      lastName: p.lastName,
      firstName: p.firstName,
      middleName: p.middleName,
      age: p.age,
      gender: p.gender || 'Male',
      maritalStatus: p.maritalStatus || 'Single',
      phoneNumber: p.phoneNumber,
      street: p.street,
      barangay: p.barangay,
      city: p.city,
      province: p.province,
      postalCode: p.postalCode,
      // Explicitly re-blank password fields after load, in case
      // the browser tries to autofill them before this runs.
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    };

    this.imagePreviewUrl = p.profileImagePath || null;
  }

  // ── Password section visibility ─────────────────────────────
  // Google account that has never set its own password: skip
  // current-password field. Everyone else (normal accounts, and
  // Google accounts that already set one) must enter it.
  get requiresCurrentPassword(): boolean {
    return this.isChangingPassword && !!this.profile?.hasPassword;
  }

  get isChangingPassword(): boolean {
    return !!this.form.newPassword || !!this.form.confirmPassword;
  }

  get passwordsMatch(): boolean {
    return this.form.newPassword === this.form.confirmPassword;
  }

  // ── Validation (touched-flag pattern, consistent with rest of app) ──
  get errors(): Record<string, string> {
    const e: Record<string, string> = {};
    if (!this.form.firstName.trim()) e['firstName'] = 'First name is required.';
    if (!this.form.lastName.trim()) e['lastName'] = 'Last name is required.';
    if (!this.form.phoneNumber.trim())
      e['phoneNumber'] = 'Phone number is required.';
    if (this.form.age <= 0) e['age'] = 'Age must be greater than 0.';
    if (!this.form.street.trim()) e['street'] = 'Street is required.';
    if (!this.form.barangay.trim()) e['barangay'] = 'Barangay is required.';
    if (!this.form.city.trim()) e['city'] = 'City is required.';
    if (!this.form.province.trim()) e['province'] = 'Province is required.';
    if (!this.form.postalCode.trim())
      e['postalCode'] = 'Postal code is required.';

    if (this.isChangingPassword) {
      if (this.requiresCurrentPassword && !this.form.currentPassword) {
        e['currentPassword'] = 'Current password is required.';
      }
      if (
        this.form.newPassword.length > 0 &&
        this.form.newPassword.length < 8
      ) {
        e['newPassword'] = 'New password must be at least 8 characters.';
      }
      if (!this.passwordsMatch) {
        e['confirmPassword'] = 'Passwords do not match.';
      }
    }

    return e;
  }

  get isValid(): boolean {
    return Object.keys(this.errors).length === 0;
  }

  hasError(field: string): boolean {
    return this.touched && !!this.errors[field];
  }

  errorMsg(field: string): string {
    return this.errors[field] ?? '';
  }

  // ── Image select ─────────────────────────────────────────────
  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    this.selectedImage = input.files[0];
    const reader = new FileReader();
    reader.onload = () => (this.imagePreviewUrl = reader.result as string);
    reader.readAsDataURL(this.selectedImage);
  }

  // ── Submit ───────────────────────────────────────────────────
  async onSubmit(): Promise<void> {
    this.touched = true;
    if (!this.isValid) return;

    this.saving = true;

    const formData = new FormData();
    formData.append('lastName', this.form.lastName);
    formData.append('firstName', this.form.firstName);
    formData.append('middleName', this.form.middleName);
    formData.append('age', this.form.age.toString());
    formData.append('gender', this.form.gender);
    formData.append('maritalStatus', this.form.maritalStatus);
    formData.append('phoneNumber', this.form.phoneNumber);
    formData.append('street', this.form.street);
    formData.append('barangay', this.form.barangay);
    formData.append('city', this.form.city);
    formData.append('province', this.form.province);
    formData.append('postalCode', this.form.postalCode);
    formData.append('currentPassword', this.form.currentPassword);
    formData.append('newPassword', this.form.newPassword);
    formData.append('confirmPassword', this.form.confirmPassword);
    if (this.selectedImage) {
      formData.append('newProfileImage', this.selectedImage);
    }

    const res = await this.profileService.updateCustomerProfileAsync(formData);

    this.saving = false;

    if (!res.isSuccess) {
      this.toastr.error(res.errorMessage || 'Failed to update profile.');
      return;
    }

    this.toastr.success(res.successMessage || 'Profile updated!');

    if (this.isChangingPassword && this.profile) {
      this.profile.hasPassword = true;
    }
    this.form.currentPassword = '';
    this.form.newPassword = '';
    this.form.confirmPassword = '';
    this.touched = false;
    this.selectedImage = null;
  }
}