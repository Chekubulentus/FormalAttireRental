import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';
import { Category } from '../../../../data/models/DTOs/Category/category';
import { CategoryService } from '../../categories/category-service/category.service'; // TODO: confirm this path
import { SupplierService } from '../suppliers-service/supplier.service';
import { AppToastrService } from '../../../../core/services/toastr-service/app-toastr.service';

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
export class AssignClothesModalComponent implements OnInit {

  // ── Inputs ────────────────────────────────────────────────────────────────
  @Input() supplierId: number | null = null;
  @Input() preSelectedIds: number[] = [];
  @Input() title = 'Assign Clothes';
  @Input() subtitle = 'Pick clothes to link to this supplier.';

  // ── Outputs ───────────────────────────────────────────────────────────────
  @Output() closeModal = new EventEmitter<void>();
  @Output() confirmSelection = new EventEmitter<AssignClothesResult>();

  constructor(
    private categoryService: CategoryService,
    private supplierService : SupplierService,
    private toastrService : AppToastrService
  ) {}

  // ── Filter State ──────────────────────────────────────────────────────────
  searchQuery    = '';
  filterCategory = '';
  filterGender   = '';

  readonly genders: string[] = ['Male', 'Female', 'Others'];

  categories: Category[] = [];

  // ── Data State ────────────────────────────────────────────────────────────
  clothes: ClotheDTO[] = [];
  totalCount = 0;
  isLoading = false;

  // ── Selection State ───────────────────────────────────────────────────────
  private originalIds = new Set<number>();
  selectedIds = new Set<number>();

  //Pagination Properties
  currentPage : number = 1;
  itemsPerPage : number = 10;

  ngOnInit(): void {
    this.originalIds = new Set(this.preSelectedIds ?? []);
    this.selectedIds = new Set(this.originalIds);

    this.loadCategories();
    this.loadClothes();
  }

  private loadCategories(): void {
    this.categoryService.getAllCategories().then(result => {
      if (result.isSuccess && result.data) {
        this.categories = result.data;
      }
    });
  }

  // ── Source & Filtering (server-side) ─────────────────────────────────────
  onFiltersChanged(): void {
    this.loadClothes();
  }

  private loadClothes(): void {
    this.isLoading = true;

    this.supplierService.filterAssignableClothesAsync(
      this.supplierId,
      this.searchQuery,
      this.filterCategory,
      this.filterGender,
      this.currentPage,
      this.itemsPerPage
    ).then(res => {
      if(!res.isSuccess) {
        this.toastrService.error(res.errorMessage ?? 'Clothes could not be fetched.');
        this.clothes = [];
        this.totalCount = 0;  
      }
      this.clothes = res.data?.clothes ?? [];
      this.totalCount = res.data?.totalCount ?? 0;
    }).catch(err => {
      this.toastrService.error(err.error);
    }).finally(() => {
      this.isLoading = false;
    });
    
    //
    // Stub for now — nothing will show until this is wired up.
    this.isLoading = false;
    this.clothes = [];
    this.totalCount = 0;
  }

  get hasActiveFilters(): boolean {
    return !!this.searchQuery.trim()
      || !!this.filterCategory
      || !!this.filterGender;
  }

  clearFilters(): void {
    this.searchQuery    = '';
    this.filterCategory = '';
    this.filterGender   = '';
    this.onFiltersChanged();
  }

  get isNoResults(): boolean {
    return this.clothes.length === 0 && this.hasActiveFilters;
  }

  get isPoolEmpty(): boolean {
    return this.clothes.length === 0 && !this.hasActiveFilters;
  }

  // ── Selection ─────────────────────────────────────────────────────────────
  isSelected(id: number): boolean            { return this.selectedIds.has(id); }
  wasOriginallyAssigned(id: number): boolean { return this.originalIds.has(id); }

  toggleClothe(id: number): void {
    if (this.selectedIds.has(id)) {
      this.selectedIds.delete(id);
    } else {
      this.selectedIds.add(id);
    }
  }

  get selectedCount(): number { return this.selectedIds.size; }

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

  clearSelection(): void { this.selectedIds.clear(); }

  // ── Modal ─────────────────────────────────────────────────────────────────
  confirm(): void {
    const assignClotheIds: number[]   = [];
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

  close(): void { this.closeModal.emit(); }

  onOverlayClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close();
    }
  }
}