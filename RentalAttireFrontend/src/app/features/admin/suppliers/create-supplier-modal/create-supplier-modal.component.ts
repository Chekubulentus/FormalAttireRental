import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface CreateSupplierForm {
  supplierName: string;
  address: string;
  phoneNumber: string;
  email: string;
}

@Component({
  selector: 'app-create-supplier-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-supplier-modal.component.html',
  styleUrl: './create-supplier-modal.component.scss'
})
export class CreateSupplierModalComponent {

  @Output() closeModal = new EventEmitter<void>();
  @Output() supplierCreated = new EventEmitter<void>(); // parent reloads table + shows toast

  // ── Form State ─────────────────────────────────────────────────────────────
  form: CreateSupplierForm = {
    supplierName: '',
    address: '',
    phoneNumber: '',
    email: '',
  };

  isSubmitting = false;
  touched = false;

  // ── Validation ─────────────────────────────────────────────────────────────
  get errors(): Record<string, string> {
    const e: Record<string, string> = {};

    if (!this.form.supplierName.trim()) {
      e['supplierName'] = 'Supplier name is required.';
    }

    if (!this.form.address.trim()) {
      e['address'] = 'Address is required.';
    }

    // Optional — validate only if filled
    if (this.form.phoneNumber.trim()) {
      const phoneRegex = /^(\+?\d[\d\s\-]{6,14}\d)$/;
      if (!phoneRegex.test(this.form.phoneNumber.trim())) {
        e['phoneNumber'] = 'Enter a valid phone number.';
      }
    }

    if (this.form.email.trim()) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(this.form.email.trim())) {
        e['email'] = 'Enter a valid email address.';
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

  onBlur(field: string): void {
    // Flip touched on first interaction so errors don't fire before the user tries
    this.touched = true;
  }

  // ── Submit ─────────────────────────────────────────────────────────────────
  async onSubmit(): Promise<void> {
    this.touched = true;

    if (!this.isValid) return;

    this.isSubmitting = true;

    try {
      // TODO: call SupplierService.createSupplierAsync(this.form)
      // const result = await this.supplierService.createSupplierAsync(this.form);
      // if (result.isSuccess) {
      //   this.supplierCreated.emit();  // parent shows toast + reloads table
      //   this.close();
      // } else {
      //   // TODO: show error toast via ToastrService
      // }
    } finally {
      this.isSubmitting = false;
    }
  }

  // ── Modal ──────────────────────────────────────────────────────────────────
  close(): void {
    this.closeModal.emit();
  }

  onOverlayClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close();
    }
  }
}