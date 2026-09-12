import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';

@Component({
  selector: 'app-view-supplier-clothes-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './view-supplier-clothes-modal.component.html',
  styleUrl: './view-supplier-clothes-modal.component.scss'
})
export class ViewSupplierClothesModalComponent implements OnInit {

  @Input() clothes: ClotheDTO[] = [];
  @Input() supplierName = '';
  @Output() closeModal = new EventEmitter<void>();

  // ── Filters ────────────────────────────────────────────────────────────────
  searchQuery = '';
  genderFilter = '';
  conditionFilter = '';

  // ── Filtered result ────────────────────────────────────────────────────────
  filteredClothes: ClotheDTO[] = [];

  // ── Pagination (client-side — data already loaded via parent) ──────────────
  currentPage = 1;
  itemsPerPage = 8;

  get totalPages(): number {
    return Math.ceil(this.filteredClothes.length / this.itemsPerPage);
  }

  get rangeStart(): number {
    return (this.currentPage - 1) * this.itemsPerPage + 1;
  }

  get rangeEnd(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.filteredClothes.length);
  }

  get paginatedClothes(): ClotheDTO[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredClothes.slice(start, start + this.itemsPerPage);
  }

  get pageNumbers(): number[] {
    const pages: number[] = [];
    const total = this.totalPages;
    const current = this.currentPage;

    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
      return pages;
    }

    pages.push(1);
    if (current > 3) pages.push(-1);
    for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
      pages.push(i);
    }
    if (current < total - 2) pages.push(-1);
    pages.push(total);

    return pages;
  }

  ngOnInit(): void {
    this.applyFilters();
  }

  // ── Filter Logic ───────────────────────────────────────────────────────────
  applyFilters(): void {
    const query = this.searchQuery.trim().toLowerCase();

    this.filteredClothes = this.clothes.filter(c => {
      const matchesSearch = !query
        || c.clotheName.toLowerCase().includes(query)
        || c.clotheCode.toLowerCase().includes(query);

      const matchesGender = !this.genderFilter
        || c.clotheGender === this.genderFilter;

      const matchesCondition = !this.conditionFilter
        || c.condition === this.conditionFilter;

      return matchesSearch && matchesGender && matchesCondition;
    });

    this.currentPage = 1;
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
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