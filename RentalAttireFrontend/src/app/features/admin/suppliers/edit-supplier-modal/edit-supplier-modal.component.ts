import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AssignClothesModalComponent, AssignClothesResult } from '../assign-clothes-modal/assign-clothes-modal.component';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';
import { SupplierDTO } from '../../../../data/models/DTOs/Supplier/supplier';

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
export class EditSupplierModalComponent implements OnChanges {

  @Input() supplier!: SupplierDTO;

  // Parent fetches unassigned clothes (SupplierId == null) and passes them in.
  // Combined with the supplier's already-assigned clothes inside this component
  // to form the full picker list.
  @Input() availableClothes: ClotheDTO[] = [];

  @Output() closeModal = new EventEmitter<void>();
  @Output() supplierUpdated = new EventEmitter<void>(); // parent reloads table + shows toast

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

  // The full list passed to the picker: supplier's current clothes + unassigned clothes.
  // Computed whenever supplier or availableClothes changes.
  pickerClothes: ClotheDTO[] = [];

  // Frozen snapshot of the supplier's originally-assigned IDs at open time.
  // Passed as preSelectedIds to AssignClothesModalComponent, which uses it
  // internally to compute its own assign/unassign diff. We store it here so
  // it doesn't shift if the parent re-inputs the supplier mid-session.
  preSelectedIdsSnapshot: number[] = [];

  // Live selection state — updated whenever the picker confirms a selection.
  // Kept as a Set for O(1) membership checks (e.g. pendingAssignCount).
  currentlySelectedIds: Set<number> = new Set();

  // The last confirmed diff from the picker — sent straight to the backend on submit.
  pendingAssignClotheIds: number[] = [];
  pendingUnassignClotheIds: number[] = [];

  // ── Lifecycle ──────────────────────────────────────────────────────────────
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['supplier'] && this.supplier) {
      this.initForm();
    }
    if (changes['supplier'] || changes['availableClothes']) {
      this.buildPickerList();
    }
  }

  private initForm(): void {
    this.form = {
      supplierName: this.supplier.supplierName,
      address:      this.supplier.address,
      phoneNumber:  this.supplier.phoneNumber ?? '',
      email:        this.supplier.email ?? '',
    };

    // Snapshot the original assignment so the picker's diff is always
    // relative to "what existed when this modal opened", not a later state.
    const originalIds = this.supplier.clothesAvailable.map(c => c.id);
    this.preSelectedIdsSnapshot  = [...originalIds];
    this.currentlySelectedIds    = new Set(originalIds);

    // Reset any pending diff from a previous open.
    this.pendingAssignClotheIds   = [];
    this.pendingUnassignClotheIds = [];

    this.touched      = false;
    this.isSubmitting = false;
  }

  private buildPickerList(): void {
    if (!this.supplier) return;

    // Merge the supplier's current clothes with externally-fetched unassigned
    // clothes, deduplicating by id in case the parent accidentally includes
    // already-assigned clothes in availableClothes.
    const assignedIds = new Set(this.supplier.clothesAvailable.map(c => c.id));
    const unassignedOnly = this.availableClothes.filter(c => !assignedIds.has(c.id));
    this.pickerClothes = [...this.supplier.clothesAvailable, ...unassignedOnly];
  }

  // TODO: replace with real ClotheService call once the "unassigned clothes"
  // query/endpoint exists on the backend (SupplierId == null filter).
  // The parent (SuppliersComponent) should call this and pass the result
  // in via [availableClothes] before opening this modal.
  // loadAvailableClothes(): void { ... }

  // ── Clothes Picker ─────────────────────────────────────────────────────────
  openAssignClothesModal(): void {
    this.isAssignClothesModalOpen = true;
  }

  // The picker already computed the assign/unassign diff internally.
  // We just store the result and update our live selection mirror.
  onClothesSelected(result: AssignClothesResult): void {
    this.pendingAssignClotheIds   = result.assignClotheIds;
    this.pendingUnassignClotheIds = result.unassignClotheIds;

    // Rebuild currentlySelectedIds from the original snapshot + diff
    // so the button label stays accurate without an extra round-trip.
    const updated = new Set(this.preSelectedIdsSnapshot);
    result.assignClotheIds.forEach(id   => updated.add(id));
    result.unassignClotheIds.forEach(id => updated.delete(id));
    this.currentlySelectedIds = updated;

    this.isAssignClothesModalOpen = false;
  }

  // ── Delta Helpers (button label) ───────────────────────────────────────────
  get pendingAssignCount(): number {
    return this.pendingAssignClotheIds.length;
  }

  get pendingUnassignCount(): number {
    return this.pendingUnassignClotheIds.length;
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

  onBlur(_field: string): void {
    this.touched = true;
  }

  // ── Submit ─────────────────────────────────────────────────────────────────
  async onSubmit(): Promise<void> {
    this.touched = true;

    if (!this.isValid) return;

    this.isSubmitting = true;

    try {
      // TODO: call SupplierService.updateSupplierAsync({
      //   supplierId:         this.supplier.id,
      //   supplierName:       this.form.supplierName,
      //   address:            this.form.address,
      //   phoneNumber:        this.form.phoneNumber,
      //   email:              this.form.email,
      //   assignClotheIds:    this.pendingAssignClotheIds,
      //   unassignClotheIds:  this.pendingUnassignClotheIds,
      // });
      // if (result.isSuccess) {
      //   this.supplierUpdated.emit();  // parent shows toast + reloads table
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