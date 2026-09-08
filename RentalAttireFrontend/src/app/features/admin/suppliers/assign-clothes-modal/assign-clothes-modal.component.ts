import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';

export interface AssignClothesResult {
  assignClotheIds: number[];
  unassignClotheIds: number[];
}

@Component({
  selector: 'app-assign-clothes-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './assign-clothes-modal.component.html',
  styleUrl: './assign-clothes-modal.component.scss'
})
export class AssignClothesModalComponent implements OnChanges {

  // ── Inputs ────────────────────────────────────────────────────────────────
  /**
   * The full pool of clothes to show in the picker.
   * - Create context: unassigned clothes only (SupplierId == null).
   * - Edit context: this supplier's currently-assigned clothes + unassigned clothes,
   *   combined by the parent before passing in.
   */
  @Input() pickerClothes: ClotheDTO[] = [];
  /**
   * Ids already assigned when the modal opens.
   * - Create context: [] (nothing pre-selected).
   * - Edit context: the supplier's current clothesAvailable ids — treated as the
   *   frozen "original" snapshot for diffing on confirm.
   */
  @Input() preSelectedIds: number[] = [];
  @Input() title = 'Assign Clothes';
  @Input() subtitle = 'Pick clothes to link to this supplier.';

  // ── Outputs ───────────────────────────────────────────────────────────────
  @Output() closeModal = new EventEmitter<void>();
  /**
   * Emits the diff against preSelectedIds, not just the raw current selection.
   * Create: unassignClotheIds is always [] (nothing to unassign from nothing),
   *   so assignClotheIds === the full picked list, matching CreateSupplierCommand.ClotheIds.
   * Edit: both arrays are populated, matching UpdateSupplierCommand's
   *   AssignClotheIds / UnassignClotheIds fields directly.
   */
  @Output() confirmSelection = new EventEmitter<AssignClothesResult>();

  // ── State ─────────────────────────────────────────────────────────────────
  searchQuery = '';
  private originalIds = new Set<number>();
  selectedIds = new Set<number>();

  ngOnChanges(changes: SimpleChanges): void {
    // Re-seed both the live selection and the frozen "original" snapshot whenever
    // the modal is (re)opened with a fresh preSelectedIds input.
    if (changes['preSelectedIds']) {
      this.originalIds = new Set(this.preSelectedIds ?? []);
      this.selectedIds = new Set(this.originalIds);
    }
  }

  // ── Filtering ─────────────────────────────────────────────────────────────
  get filteredClothes(): ClotheDTO[] {
    const query = this.searchQuery.trim().toLowerCase();
    if (!query) return this.pickerClothes;

    return this.pickerClothes.filter(clothe =>
      clothe.clotheName.toLowerCase().includes(query) ||
      clothe.clotheCode.toLowerCase().includes(query)
    );
  }

  // ── Selection ─────────────────────────────────────────────────────────────
  isSelected(id: number): boolean {
    return this.selectedIds.has(id);
  }

  /** True if this clothe was already assigned to the supplier before this session opened. */
  wasOriginallyAssigned(id: number): boolean {
    return this.originalIds.has(id);
  }

  toggleClothe(id: number): void {
    if (this.selectedIds.has(id)) {
      this.selectedIds.delete(id);
    } else {
      this.selectedIds.add(id);
    }
  }

  get selectedCount(): number {
    return this.selectedIds.size;
  }

  get newlyAssignedCount(): number {
    let count = 0;
    this.selectedIds.forEach(id => { if (!this.originalIds.has(id)) count++; });
    return count;
  }

  get newlyUnassignedCount(): number {
    let count = 0;
    this.originalIds.forEach(id => { if (!this.selectedIds.has(id)) count++; });
    return count;
  }

  clearSelection(): void {
    this.selectedIds.clear();
  }

  // ── Modal ─────────────────────────────────────────────────────────────────
  confirm(): void {
    const assignClotheIds: number[] = [];
    const unassignClotheIds: number[] = [];

    this.selectedIds.forEach(id => {
      if (!this.originalIds.has(id)) assignClotheIds.push(id);
    });
    this.originalIds.forEach(id => {
      if (!this.selectedIds.has(id)) unassignClotheIds.push(id);
    });

    this.confirmSelection.emit({ assignClotheIds, unassignClotheIds });
    this.close();
  }

  close(): void {
    this.closeModal.emit();
  }

  onOverlayClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close();
    }
  }
}