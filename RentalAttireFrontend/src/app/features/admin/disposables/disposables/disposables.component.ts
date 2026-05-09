import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DisposablesService } from '../disposables-service/disposables.service';
import { AppToastrService } from '../../../../core/services/toastr-service/app-toastr.service';
import { UserService } from '../../../../core/services/user-service/user.service';
import { UserViewModel } from '../../../../data/models/DTOs/Users/user-view-model';

// ── Inline model — replace with real import once created ─────
export class ArchivedEntityDto {
  entityType: string      = '';
  entityId: number        = 0;
  archivedAt: Date | null = null;
  archivedBy: string      = '';
}

export class RestoreRecordCommand {
  id: number            = 0;
  entityType: string    = '';
  performedBy: string   = '';
  performedById: number = 0;
}
// ─────────────────────────────────────────────────────────────

@Component({
  selector: 'app-disposables',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './disposables.component.html',
  styleUrl: './disposables.component.scss',
})
export class DisposablesComponent implements OnInit {

  // ============================================================
  // State
  // ============================================================
  isLoading = false;
  records: ArchivedEntityDto[] = [];

  // ── Search & Filter ────────────────────────────────────────
  searchQuery = '';
  filterType  = ''; // 'Employee' | 'Customer' | 'Clothe' | 'Category' | ''

  // ── Pagination ─────────────────────────────────────────────
  currentPage  = 1;
  itemsPerPage = 10;
  totalCount   = 0;
  totalPages   = 1;

  // ── Confirmation modal ─────────────────────────────────────
  pendingRecord: ArchivedEntityDto | null     = null;
  pendingAction: 'restore' | 'delete' | null  = null;
  isConfirming = false;

  // ── View modal ─────────────────────────────────────────────
  recordToView: ArchivedEntityDto | null = null;

  // ── Current user ───────────────────────────────────────────
  currentUser: UserViewModel | undefined;

  constructor(
    private disposablesService: DisposablesService,
    private toastrService: AppToastrService,
    private userService: UserService,
  ) {}

  ngOnInit(): void {
    this.getCurrentUser();
    this.getAllArchivedRecords();
  }


  // ============================================================
  // Data Loading
  // ============================================================
  getAllArchivedRecords(): void {
    this.isLoading = true;

    this.disposablesService
      .getAllArchivedRecordsAsync(this.currentPage, this.itemsPerPage)
      .then((res) => {
        if (!res.isSuccess) {
          this.records    = [];
          this.totalCount = 0;
          this.totalPages = 1;
          this.toastrService.error(res.errorMessage ?? 'Records could not be fetched.');
          return;
        }
        this.records    = res.data?.items ?? [];
        this.totalCount = res.data?.totalCount ?? 0;
        this.totalPages = res.data?.totalPages ?? 1;
      })
      .catch((err) => {
        this.toastrService.error(err.error);
      })
      .finally(() => {
        this.isLoading = false;
      });
  }


  // ============================================================
  // Search & Filter
  // ============================================================
  get filteredRecords(): ArchivedEntityDto[] {
    let list = this.records;
    if (this.filterType)
      list = list.filter((r) => r.entityType === this.filterType);
    if (this.searchQuery.trim())
      list = list.filter(
        (r) =>
          r.entityType.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
          r.archivedBy.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
          r.entityId.toString().includes(this.searchQuery),
      );
    return list;
  }

  get hasActiveFilters(): boolean {
    return !!this.searchQuery.trim() || !!this.filterType;
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.filterType  = '';
  }

  // ── Per-type counts ────────────────────────────────────────
  get employeeCount(): number { return this.records.filter((r) => r.entityType === 'Employee').length; }
  get customerCount(): number { return this.records.filter((r) => r.entityType === 'Customer').length; }
  get clotheCount():   number { return this.records.filter((r) => r.entityType === 'Clothe').length; }
  get categoryCount(): number { return this.records.filter((r) => r.entityType === 'Category').length; }


  // ============================================================
  // Pagination
  // ============================================================
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.getAllArchivedRecords();
  }

  get rangeStart(): number {
    return Math.min((this.currentPage - 1) * this.itemsPerPage + 1, this.totalCount);
  }

  get rangeEnd(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.totalCount);
  }

  get pageNumbers(): number[] {
    const total = this.totalPages, current = this.currentPage;
    const range: number[] = [], pages: number[] = [];
    for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++)
      range.push(i);
    pages.push(1);
    if (range.length > 0 && range[0] > 2) pages.push(-1);
    pages.push(...range);
    if (range.length > 0 && range[range.length - 1] < total - 1) pages.push(-1);
    if (total > 1) pages.push(total);
    return pages;
  }


  // ============================================================
  // View Modal
  // ============================================================
  openViewModal(record: ArchivedEntityDto): void { this.recordToView = record; }
  closeViewModal(): void                          { this.recordToView = null; }


  // ============================================================
  // Confirmation Modal
  // ============================================================
  openConfirm(record: ArchivedEntityDto, action: 'restore' | 'delete'): void {
    this.pendingRecord = record;
    this.pendingAction = action;
  }

  closeConfirm(): void {
    this.pendingRecord = null;
    this.pendingAction = null;
  }

  onConfirmed(): void {
    if (!this.pendingRecord || !this.pendingAction) return;
  }


  // ============================================================
  // Restore
  // ============================================================


  // ============================================================
  // Permanent Delete
  // ============================================================


  // ============================================================
  // Helpers
  // ============================================================
  getEntityIcon(entityType: string): string {
    const map: Record<string, string> = {
      Employee: 'person',
      Customer: 'customer',
      Clothe:   'clothe',
      Category: 'category',
    };
    return map[entityType] ?? 'default';
  }

  getEntityTypeClass(entityType: string): string {
    const map: Record<string, string> = {
      Employee: 'type--employee',
      Customer: 'type--customer',
      Clothe:   'type--clothe',
      Category: 'type--category',
    };
    return map[entityType] ?? '';
  }

  getCurrentUser(): void {
    this.isLoading = true;
    this.userService
      .getCurrentUserViewModel()
      .then((res) => {
        if (!res.isSuccess)
          console.log(`${res.errorMessage} ?? Current user cannot be found.`);
        this.currentUser = res.data ?? undefined;
      })
      .catch((err) => {
        this.toastrService.error(err.error);
      })
      .finally(() => {
        this.isLoading = false;
      });
  }
}