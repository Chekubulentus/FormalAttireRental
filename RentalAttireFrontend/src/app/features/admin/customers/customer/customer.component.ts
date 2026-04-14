import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Customer } from '../../../../data/models/DTOs/Customer/customer';
import { ToastrService } from 'ngx-toastr';
import { CustomerService } from '../customer-service/customer.service';
import { ViewCustomerComponent } from '../view-customer/view-customer.component';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CurrencyPipe,
    ViewCustomerComponent
    // ArchiveConfirmationComponent,
  ],
  templateUrl: './customer.component.html',
  styleUrl: './customer.component.scss',
})
export class CustomerComponent implements OnInit {

  // ============================================================
  // State
  // ============================================================
  isLoading = false;
  customers: Customer[] = [];

  // ── Search & Filters ───────────────────────────────────────
  searchQuery     = '';
  filterType      = ''; // 'google' | 'regular' | ''

  // ── Pagination ─────────────────────────────────────────────
  currentPage  = 1;
  itemsPerPage = 10;
  totalCount   = 0;
  totalPages   = 1;

  // ── Modals ─────────────────────────────────────────────────
  customerToView: Customer | null = null;
  customerToArchive: Customer | undefined = undefined;
  isArchiving = false;

  // ── Avatar colors ──────────────────────────────────────────
  private avatarColors = [
    '#a07840', '#7a9e7e', '#8b7aad', '#4a90a4',
    '#c0697a', '#6b8e6b', '#a0522d', '#5a7a9e',
  ];

  constructor(
    private toastrService : ToastrService,
    private customerService : CustomerService
  ) {}

  ngOnInit(): void {
    this.getAllCustomers();
  }

  // ============================================================
  // Data Loading
  // ============================================================
  getAllCustomers(): void {
    this.isLoading = true;
    
    this.customerService.filterClothesAsync(
      this.currentPage,
      this.itemsPerPage,
      this.searchQuery
    ).then(res => {
      if(!res.isSuccess)
        this.toastrService.error(res.errorMessage ?? 'No customers found.');
      this.customers = res.data?.items ?? [];
      this.totalCount = res.data?.totalCount ?? 0;
    }).catch(err => {
      this.toastrService.error(err.error);
    }).finally(() => {
      this.isLoading = false;
    })

    this.totalCount = 2;
    this.totalPages = 1;
    this.isLoading  = false;
  }

  // ============================================================
  // Search & Filter
  // ============================================================
  onSearch(): void {
    this.currentPage = 1;
    this.getAllCustomers();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.getAllCustomers();
  }

  get hasActiveFilters(): boolean {
    return !!this.searchQuery.trim() || !!this.filterType;
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.filterType  = '';
    this.currentPage = 1;
    this.getAllCustomers();
  }

  // ============================================================
  // Pagination
  // ============================================================
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.getAllCustomers();
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
  // Stats
  // ============================================================
  get totalSpentAll(): number {
    return this.customers.reduce((sum, c) => sum + c.totalSpent, 0);
  }

  get totalRentalsAll(): number {
    return this.customers.reduce((sum, c) => sum + c.totalRentals, 0);
  }

  // ============================================================
  // Modals
  // ============================================================
  openViewModal(customer: Customer): void    { 
    this.customerToView = customer; 
  }
  closeViewModal(): void                         { this.customerToView = null; }

  openArchiveModal(customer: Customer): void  { this.customerToArchive = customer; }
  closeArchiveModal(): void                      { this.customerToArchive = undefined; }

  onArchiveConfirmed(): void {
    if (!this.customerToArchive) return;
    this.isArchiving = true;
    // TODO: wire to CustomerService.archiveCustomerAsync()
    // this.customerService.archiveCustomerAsync(this.customerToArchive.id)
    //   .then(res => {
    //     if (!res.isSuccess) { this.toastr.error('Failed to archive customer.'); return; }
    //     this.toastr.success('Customer archived.');
    //     this.getAllCustomers();
    //   })
    //   .catch(err => console.error(err))
    //   .finally(() => { this.isArchiving = false; this.closeArchiveModal(); });
    this.isArchiving = false;
    this.closeArchiveModal();
  }

  // ============================================================
  // Helpers
  // ============================================================
  getAvatarColor(id: number): string {
    return this.avatarColors[id % this.avatarColors.length];
  }

  getInitials(fullName: string): string {
    const parts = fullName.trim().split(' ');
    if (parts.length === 1) return parts[0][0]?.toUpperCase() ?? '?';
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }

  onImgError(event: Event, customer: Customer): void {
    (event.target as HTMLImageElement).style.display = 'none';
    customer.person.profileImagePath = '';
  }
}