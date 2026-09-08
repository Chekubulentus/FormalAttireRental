import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CreateSupplierModalComponent } from '../create-supplier-modal/create-supplier-modal.component';

// TODO: Replace with real DTO from src/app/data/models/DTOs/Supplier/supplier.ts
interface SupplierDTO {
  id: number;
  supplierCode: string;
  supplierName: string;
  phoneNumber: string;
  email: string;
  address: string;
  clothesAvailable: any[];
  isArchived: boolean;
}

@Component({
  selector: 'app-suppliers',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule,
    CreateSupplierModalComponent
  ],
  templateUrl: './suppliers.component.html',
  styleUrl: './suppliers.component.scss'
})
export class SuppliersComponent implements OnInit {

  // ── State ──────────────────────────────────────────────────────────────────
  suppliers: SupplierDTO[] = [];
  isLoading = false;

  // ── Filters ────────────────────────────────────────────────────────────────
  searchQuery = '';
  statusFilter = ''; // '' | 'active' | 'archived'

  // ── Stats ──────────────────────────────────────────────────────────────────
  totalCount = 0;
  activeCount = 0;
  archivedCount = 0;

  // ── Pagination ─────────────────────────────────────────────────────────────
  currentPage = 1;
  itemsPerPage = 10;
  totalPages = 0;

  //Create Supplier Modal Properties
  openCreateSupplierModal : boolean = false;

  get rangeStart(): number {
    return (this.currentPage - 1) * this.itemsPerPage + 1;
  }

  get rangeEnd(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.totalCount);
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
    // TODO: wire up SupplierService.getSuppliersAsync(...)
  }

  // ── Filter Handlers ────────────────────────────────────────────────────────
  onSearchChange(): void {
    this.currentPage = 1;
    // TODO: call loadSuppliers()
  }

  onFilterChange(): void {
    this.currentPage = 1;
    // TODO: call loadSuppliers()
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    // TODO: call loadSuppliers()
  }

  // ── Modal Triggers (wired up when modals are built) ────────────────────────
  openAddModal(): void {
    this.openCreateSupplierModal = true;
  }

  openEditModal(supplier: SupplierDTO): void {
    // TODO: open EditSupplierModalComponent with supplier
  }

  openViewModal(supplier: SupplierDTO): void {
    // TODO: open ViewSupplierModalComponent with supplier
  }

  openAssignClothesModal(supplier: SupplierDTO): void {
    // TODO: open AssignClothesModalComponent with supplier
  }

  // ── Archive / Restore ──────────────────────────────────────────────────────
  archiveSupplier(supplier: SupplierDTO): void {
    // TODO: call SupplierService.archiveSupplier(supplier.id)
  }

  restoreSupplier(supplier: SupplierDTO): void {
    // TODO: call SupplierService.restoreSupplier(supplier.id)
  }
}