import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AssignClothesModalComponent, AssignClothesResult } from '../assign-clothes-modal/assign-clothes-modal.component';
import { SupplierDTO } from '../../../../data/models/DTOs/Supplier/supplier';
import { SupplierService } from '../suppliers-service/supplier.service';
import { AppToastrService } from '../../../../core/services/toastr-service/app-toastr.service';

interface EditSupplierForm {
  supplierName: string;
  address: string;
  phoneNumber: string;
  email: string;
}

@Component({
  selector: 'app-edit-supplier-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, AssignClothesModalComponent],
  templateUrl: './edit-supplier-modal.component.html',
  styleUrl: './edit-supplier-modal.component.scss'
})
export class EditSupplierModalComponent implements OnInit {

  @Input() supplierId!: number;

  @Output() closeModal = new EventEmitter<void>();
  @Output() supplierUpdated = new EventEmitter<void>();

  constructor(
    private supplierService: SupplierService,
    private toastrService: AppToastrService
  ) {}

  // ── Remote State ───────────────────────────────────────────────────────────
  supplier: SupplierDTO | null = null;
  isLoading = false;

  // ── Form State ─────────────────────────────────────────────────────────────
  form: EditSupplierForm = {
    supplierName: '',
    address: '',
    phoneNumber: '',
    email: '',
  };

  isSubmitting = false;
  touched = false;

  // ── Clothes Picker State ───────────────────────────────────────────────────
  isAssignClothesModalOpen = false;

  // Frozen snapshot of originally-assigned IDs — passed to AssignClothesModalComponent
  // as its diff baseline. Set once when the supplier loads; never changes mid-session.
  preSelectedIdsSnapshot: number[] = [];

  // Live mirror of the current selection — updated when the picker confirms.
  // Used only for the button label.
  currentlySelectedIds: Set<number> = new Set();

  // The last confirmed diff from the picker — sent to the backend on submit.
  pendingAssignClotheIds: number[] = [];
  pendingUnassignClotheIds: number[] = [];

  // ── Lifecycle ──────────────────────────────────────────────────────────────
  ngOnInit(): void {
    this.loadSupplier();
  }

  private loadSupplier(): void {
    this.isLoading = true;

    this.supplierService.getSupplierByIdAsync(this.supplierId)
      .then(res => {
        if (!res.isSuccess || !res.data) {
          this.toastrService.error(res.errorMessage ?? 'Supplier could not be loaded.');
          this.close();
          return;
        }

        this.supplier = res.data;
        this.initForm();
      })
      .catch(err => {
        this.toastrService.error(err.error);
        this.close();
      })
      .finally(() => {
        this.isLoading = false;
      });
  }

  private initForm(): void {
    if (!this.supplier) return;

    this.form = {
      supplierName: this.supplier.supplierName,
      address:      this.supplier.address,
      phoneNumber:  this.supplier.phoneNumber ?? '',
      email:        this.supplier.email ?? '',
    };

    // Snapshot the original assignment so the picker's diff is always
    // relative to "what existed when this modal opened".
    const originalIds = this.supplier.clothesAvailable.map(c => c.id);
    this.preSelectedIdsSnapshot = [...originalIds];
    this.currentlySelectedIds   = new Set(originalIds);

    // Reset any pending diff from a previous open.
    this.pendingAssignClotheIds   = [];
    this.pendingUnassignClotheIds = [];

    this.touched      = false;
    this.isSubmitting = false;
  }

  // ── Clothes Picker ─────────────────────────────────────────────────────────
  openAssignClothesModal(): void {
    this.isAssignClothesModalOpen = true;
  }

  onClothesSelected(result: AssignClothesResult): void {
    this.pendingAssignClotheIds   = result.assignClotheIds;
    this.pendingUnassignClotheIds = result.unassignClotheIds;

    // Rebuild the live mirror from the snapshot + confirmed diff.
    const updated = new Set(this.preSelectedIdsSnapshot);
    result.assignClotheIds.forEach(id   => updated.add(id));
    result.unassignClotheIds.forEach(id => updated.delete(id));
    this.currentlySelectedIds = updated;

    this.isAssignClothesModalOpen = false;
  }

  // ── Delta Helpers (button label) ───────────────────────────────────────────
  get pendingAssignCount(): number   { return this.pendingAssignClotheIds.length; }
  get pendingUnassignCount(): number { return this.pendingUnassignClotheIds.length; }

  // ── Validation ─────────────────────────────────────────────────────────────
  get errors(): Record<string, string> {
    const e: Record<string, string> = {};

    if (!this.form.supplierName.trim())
      e['supplierName'] = 'Supplier name is required.';

    if (!this.form.address.trim())
      e['address'] = 'Address is required.';

    if (this.form.phoneNumber.trim()) {
      const phoneRegex = /^(\+?\d[\d\s\-]{6,14}\d)$/;
      if (!phoneRegex.test(this.form.phoneNumber.trim()))
        e['phoneNumber'] = 'Enter a valid phone number.';
    }

    if (this.form.email.trim()) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(this.form.email.trim()))
        e['email'] = 'Enter a valid email address.';
    }

    return e;
  }

  get isValid(): boolean { return Object.keys(this.errors).length === 0; }

  hasError(field: string): boolean { return this.touched && !!this.errors[field]; }
  errorMsg(field: string): string  { return this.errors[field] ?? ''; }

  onBlur(_field: string): void { this.touched = true; }

  // ── Submit ─────────────────────────────────────────────────────────────────
  async onSubmit(): Promise<void> {
    this.touched = true;
    if (!this.isValid || !this.supplier) return;

    this.isSubmitting = true;

    try {
      const result = await this.supplierService.updateSupplierAsync({
        supplierId:        this.supplier.id,
        supplierName:      this.form.supplierName,
        address:           this.form.address,
        phoneNumber:       this.form.phoneNumber,
        email:             this.form.email,
        assignClotheIds:   this.pendingAssignClotheIds,
        unassignClotheIds: this.pendingUnassignClotheIds,
      });

      if (result.isSuccess) {
        this.supplierUpdated.emit();
        this.close();
      } else {
        this.toastrService.error(result.errorMessage ?? 'Supplier could not be updated.');
      }
    } catch (err: any) {
      this.toastrService.error(err.error);
    } finally {
      this.isSubmitting = false;
    }
  }

  // ── Modal ──────────────────────────────────────────────────────────────────
  close(): void { this.closeModal.emit(); }

  onOverlayClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close();
    }
  }
}