import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AssignClothesModalComponent, AssignClothesResult } from '../assign-clothes-modal/assign-clothes-modal.component';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';
import { SupplierService } from '../suppliers-service/supplier.service';
import { CreateSupplierCommand } from '../dtos/create-supplier-command';

interface CreateSupplierForm {
  supplierName: string;
  address: string;
  phoneNumber: string;
  email: string;
}

@Component({
  selector: 'app-create-supplier-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, AssignClothesModalComponent],
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

  // ── Assign Clothes State ───────────────────────────────────────────────────
  isAssignClothesModalOpen = false;
  availableClothes: ClotheDTO[] = [];
  selectedClotheIds: number[] = [];

  constructor(
    private supplierService: SupplierService,
    private toastr: ToastrService
  ) {
    this.loadAvailableClothes();
  }

  // TODO: replace with real ClotheService call once the "unassigned clothes"
  // query/endpoint exists on the backend (SupplierId == null filter).
  loadAvailableClothes(): void {
    // const result = await this.clotheService.getUnassignedClothesAsync();
    // if (result.isSuccess) this.availableClothes = result.data;
    this.availableClothes = [];
  }

  openAssignClothesModal(): void {
    this.isAssignClothesModalOpen = true;
  }

  // Create only ever has something to assign — originalIds inside the picker
  // starts empty, so unassignClotheIds will always come back [] here.
  onClothesSelected(result: AssignClothesResult): void {
    this.selectedClotheIds = result.assignClotheIds;
    this.isAssignClothesModalOpen = false;
  }

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
      const command: CreateSupplierCommand = {
        supplierName: this.form.supplierName.trim(),
        address: this.form.address.trim(),
        phoneNumber: this.form.phoneNumber.trim() || undefined,
        email: this.form.email.trim() || undefined,
        clotheIds: this.selectedClotheIds
      };

      const result = await this.supplierService.createSupplierAsync(command);

      if (result.isSuccess) {
        this.toastr.success(result.successMessage);
        this.supplierCreated.emit();  // parent shows toast + reloads table
        this.close();
      } else {
        this.toastr.error(result.errorMessage ?? 'Failed to create supplier.', 'Error');
      }
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