import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CreateSupplierModalComponent } from '../create-supplier-modal/create-supplier-modal.component';
import { SupplierDTO } from '../../../../data/models/DTOs/Supplier/supplier';
import { ArchiveConfirmationComponent } from '../../../../shared/components/archive-confirmation/archive-confirmation/archive-confirmation.component';
import { SupplierService } from '../suppliers-service/supplier.service';
import { ToastrService } from 'ngx-toastr';
import { EditSupplierModalComponent } from '../edit-supplier-modal/edit-supplier-modal.component';
import { ViewSupplierModalComponent } from '../view-supplier-modal/view-supplier-modal.component';

@Component({
  selector: 'app-suppliers',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CreateSupplierModalComponent,
    ArchiveConfirmationComponent,
    EditSupplierModalComponent,
    ViewSupplierModalComponent
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

  // ── Pagination ─────────────────────────────────────────────────────────────
  currentPage = 1;
  itemsPerPage = 10;
  totalPages = 0;

  //Create Supplier Modal Properties
  openCreateSupplierModal : boolean = false;

  //Archive Confirmation Properties
  supplierToArchive : SupplierDTO | null = null;
  isArchiving: boolean = false;

  //Edit Supplier Properties
  supplierToEdit : SupplierDTO | null = null;

  //View Supplier Properties
  supplierToView : SupplierDTO | null = null;

  constructor(
    private supplierService : SupplierService,
    private toastrService : ToastrService
  ) {}

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
    this.filterSuppliers();
  }

  // ── Filter Handlers ────────────────────────────────────────────────────────
  onSearchChange(): void {
    this.currentPage = 1;
    this.filterSuppliers();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    // TODO: call filterSuppliers() once a status filter control exists
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.filterSuppliers();
  }

  // ── Modal Triggers (wired up when modals are built) ────────────────────────
  openAddModal(): void {
    this.openCreateSupplierModal = true;
  }

  closeAddModal(): void {
    this.openCreateSupplierModal = false;
  }

  supplierCreated() {
    this.filterSuppliers();
    this.openCreateSupplierModal = false;
  }

  openEditModal(supplier: SupplierDTO): void {
    // TODO: open EditSupplierModalComponent with supplier
    this.supplierToEdit = supplier;
  }

  closeEditModal() {
    this.supplierToEdit = null;
  }

  onSupplierUpdated(): void {
    this.toastrService.success('Supplier updated successfully.');
    this.closeEditModal();
    this.filterSuppliers();
  }

  openViewModal(supplier: SupplierDTO): void {
    this.supplierToView = supplier;
  }

  closeViewModal() {
    this.supplierToView = null;
  }

  // ── Archive / Restore ──────────────────────────────────────────────────────
  archiveSupplier(supplier: SupplierDTO): void {
    // TODO: call SupplierService.archiveSupplier(supplier.id)
  }

  restoreSupplier(supplier: SupplierDTO): void {
    // TODO: call SupplierService.restoreSupplier(supplier.id)
  }

  filterSuppliers() {
    this.isLoading = true;
    this.supplierService.filterSuppliersAsync(
      this.currentPage,
      this.itemsPerPage,
      this.searchQuery
    ).then(res => {
      if (!res.isSuccess) {
        this.suppliers = [];
        this.totalCount = 0;
        this.totalPages = 0;
        return;
      }

      this.suppliers = res.data?.items ?? [];
      this.totalCount = res.data?.totalCount ?? 0;
      this.totalPages = res.data?.totalPages ?? 0;
    }).catch(err => {
      this.toastrService.error(err.error);
    }).finally(() => {
      this.isLoading = false;
    })
  }

  openArchiveModal(supplier : SupplierDTO) {
    this.supplierToArchive = supplier;
  }

  closeArchiveModal() {
    this.supplierToArchive = null;
  }

  onArchiveConfirmed() {

  }

}