import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';
import { ClotheService } from '../clothe-service/clothe.service';
import { ToastrService } from 'ngx-toastr';

// TODO: import your ClotheDTO and ClotheService
// import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothe-dto';
// import { ClotheService } from '../clothe-service/clothe.service';

@Component({
  selector: 'app-clothes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './clothes.component.html',
  styleUrl: './clothes.component.scss',
})
export class ClothesComponent implements OnInit {

  // ============================================================
  // State
  // ============================================================
  isLoading = false;
  clothes: ClotheDTO[] = [];

  // ── Search & Filters ───────────────────────────────────────
  searchQuery     = '';
  filterCategory  = '';
  filterGender    = '';
  filterCondition = '';

  // ── Pagination ─────────────────────────────────────────────
  currentPage  = 1;
  itemsPerPage = 12;
  totalCount   = 0;
  totalPages   = 1;

  // ── Modals ─────────────────────────────────────────────────
  showAddModal    = false;
  clotheToEdit: ClotheDTO | null = null;
  clotheToArchive: ClotheDTO | null = null;
  clotheToView: ClotheDTO | null = null;

  // ── Dropdown options ───────────────────────────────────────
  categories: string[] = [
    'Barong', 'Gown', 'Suit', 'Groom', 'Bridesmaid',
    'Debut', 'Costume', 'Casual', 'Others',
  ];

  genders: string[] = ['Male', 'Female', 'Unisex'];

  conditions: string[] = ['New', 'Good', 'Fair', 'Poor'];

  // constructor(private clotheService: ClotheService) {}

  constructor (
    private clotheService : ClotheService,
    private toastr : ToastrService
  ) {}

  ngOnInit(): void {
    this.getAllClothes();
  }

  // ============================================================
  // Data Loading — TODO: wire to your ClotheService
  // ============================================================
  getAllClothes(): void {
    this.isLoading = true;

    this.clotheService.filterClothesAsync(
      this.searchQuery,
      this.filterCondition,
      this.filterGender,
      this.filterCategory,
      this.currentPage,
      this.itemsPerPage
    ).then(res => {
      console.log('Category Filter: ' + this.filterCategory);
      this.clothes = res.data?.items ?? [];
      this.totalCount = res.data?.totalCount ?? 0;
      this.totalPages = res.data?.totalPages ?? 0;
    }).catch(err => {
      this.toastr.error(err.error);
    }).finally(() => this.isLoading = false);
  }

  // ============================================================
  // Search & Filter
  // ============================================================
  onSearch(): void {
    this.currentPage = 1;
    this.getAllClothes();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.getAllClothes();
  }

  get hasActiveFilters(): boolean {
    return !!this.searchQuery.trim()
      || !!this.filterCategory
      || !!this.filterGender
      || !!this.filterCondition;
  }

  clearFilters(): void {
    this.searchQuery     = '';
    this.filterCategory  = '';
    this.filterGender    = '';
    this.filterCondition = '';
    this.currentPage     = 1;
    this.getAllClothes();
  }

  // ============================================================
  // Stats
  // ============================================================
  get avgRentalPrice(): number {
    if (this.clothes.length === 0) return 0;
    const total = this.clothes.reduce((sum, c) => sum + c.rentalPrice, 0);
    return Math.round(total / this.clothes.length);
  }

  // ============================================================
  // Pagination
  // ============================================================
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.getAllClothes();
  }

  get rangeStart(): number {
    return Math.min((this.currentPage - 1) * this.itemsPerPage + 1, this.totalCount);
  }

  get rangeEnd(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.totalCount);
  }

  get pageNumbers(): number[] {
    const total   = this.totalPages;
    const current = this.currentPage;
    const range: number[] = [];
    const pages: number[] = [];

    for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
      range.push(i);
    }

    pages.push(1);
    if (range.length > 0 && range[0] > 2) pages.push(-1);
    pages.push(...range);
    if (range.length > 0 && range[range.length - 1] < total - 1) pages.push(-1);
    if (total > 1) pages.push(total);

    return pages;
  }

  // ============================================================
  // Modal Handlers
  // ============================================================
  openAddModal(): void              { this.showAddModal = true; }
  closeAddModal(): void             { this.showAddModal = false; }

  openEditModal(c: ClotheDTO): void    { this.clotheToEdit = c; }
  closeEditModal(): void               { this.clotheToEdit = null; }

  openArchiveModal(c: ClotheDTO): void { this.clotheToArchive = c; }
  closeArchiveModal(): void            { this.clotheToArchive = null; }

  openViewModal(c: ClotheDTO): void    { this.clotheToView = c; }
  closeViewModal(): void               { this.clotheToView = null; }

  onClotheUpdated(updated: ClotheDTO): void {
    const index = this.clothes.findIndex(c => c.clotheCode === updated.clotheCode);
    if (index !== -1) this.clothes[index] = updated;
    this.closeEditModal();
    // TODO: this.toastr.success('Item successfully updated.');
  }

  onArchiveConfirmed(): void {
    // TODO: call archive API then toastr
    this.closeArchiveModal();
  }

  onAddToRental(c: ClotheDTO): void {
    // TODO: emit to parent or navigate to rental creation with this item
    console.log('Add to rental:', c.clotheCode);
  }

  // ============================================================
  // Helpers
  // ============================================================
  getStockStatus(c: ClotheDTO): 'available' | 'low' | 'out' {
    if (c.availableQuantity === 0)                    return 'out';
    if (c.availableQuantity <= c.stockQuantity * 0.2) return 'low';
    return 'available';
  }

  getConditionClass(condition: string): string {
    switch (condition.toLowerCase()) {
      case 'new':  return 'condition--new';
      case 'good': return 'condition--good';
      case 'fair': return 'condition--fair';
      case 'poor': return 'condition--poor';
      default:     return '';
    }
  }

  onImgError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }
}