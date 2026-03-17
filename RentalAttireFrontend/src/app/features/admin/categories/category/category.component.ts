import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../category-service/category.service';
import { ToastrService } from 'ngx-toastr';

// TODO: replace with your actual imports
// import { Category } from '../../../../data/models/DTOs/Categories/category';
// import { CategoryService } from '../category-service/category.service';
// import { AppToastrService } from '../../../../core/services/toastr-service/app-toastr.service';

export class Category {
  id: number = 0;
  categoryCode: string = '';
  categoryName: string = '';
  description: string = '';
}

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category.component.html',
  styleUrl: './category.component.scss',
})
export class CategoryComponent implements OnInit {

  // ============================================================
  // State
  // ============================================================
  isLoading    = false;
  isSubmitting = false;
  categories: Category[] = [];

  // ── Inline create form ─────────────────────────────────────
  // showCreateRow flips to true when the user clicks "+ Add Category"
  showCreateRow = false;
  createTouched = false;

  newCategory: Category = new Category();

  // ── Edit modal ─────────────────────────────────────────────
  categoryToEdit: Category | null = null;

  // ── Archive confirmation ───────────────────────────────────
  categoryToArchive: Category | null = null;
  isArchiving = false;

  // ── Search & Pagination ────────────────────────────────────
  searchQuery  = '';
  currentPage  = 1;
  itemsPerPage = 10;
  totalCount   = 0;
  totalPages   = 1;

  // constructor(
  //   private categoryService: CategoryService,
  //   private toastr: AppToastrService,
  // ) {}

  constructor(
    private categoryService : CategoryService,
    private toastrService : ToastrService
  ) {}

  ngOnInit(): void {
    this.getAllCategories();
  }

  // ============================================================
  // Data Loading — TODO: wire to your CategoryService
  // ============================================================
  getAllCategories(): void {
    this.isLoading = true;

    this.categoryService.filterCategoriesAsync(
      this.searchQuery,
      this.currentPage,
      this.itemsPerPage
    ).then(res => {
      if(!res.isSuccess)
        console.log(`${res.errorMessage ?? 'Categories not found.'}`);
      this.categories = res.data?.items ?? [];
    }).catch(err => {
      console.log(`${err.error}`);
      this.toastrService.error(JSON.stringify(err.error));
    }).finally(() => {
      this.isLoading = false;
    });
  }

  // ============================================================
  // Search
  // ============================================================
  onSearch(): void {
    this.currentPage = 1;
    this.getAllCategories();
  }

  // ============================================================
  // Inline Create Form
  // ============================================================

  // Validation for the inline create form
  get createErrors(): Record<string, string> {
    const e: Record<string, string> = {};
    if (!this.newCategory.categoryCode?.trim()) e['categoryCode'] = 'Code is required.';
    if (!this.newCategory.categoryName?.trim()) e['categoryName'] = 'Name is required.';
    return e;
  }

  get createIsValid(): boolean {
    return Object.keys(this.createErrors).length === 0;
  }

  hasCreateError(field: string): boolean {
    return this.createTouched && !!this.createErrors[field];
  }

  openCreateRow(): void {
    this.newCategory   = new Category();
    this.createTouched = false;
    this.showCreateRow = true;
  }

  cancelCreate(): void {
    this.showCreateRow = false;
    this.createTouched = false;
    this.newCategory   = new Category();
  }

  saveCategory(): void {
    this.createTouched = true;
    if (!this.createIsValid) return;

    this.isSubmitting = true;
    // TODO:
    // this.categoryService.createCategoryAsync(this.newCategory)
    //   .then(res => {
    //     if (!res.isSuccess) {
    //       this.toastr.error(res.errorMessage ?? 'Failed to create category.');
    //       return;
    //     }
    //     this.toastr.success('Category successfully created.');
    //     this.cancelCreate();
    //     this.getAllCategories();
    //   })
    //   .catch(err => console.error(err))
    //   .finally(() => this.isSubmitting = false);
    this.isSubmitting = false;
    this.cancelCreate();
  }

  // ============================================================
  // Edit Modal
  // ============================================================
  openEditModal(category: Category): void {
    // Deep copy so edits don't affect the list until saved
    this.categoryToEdit = { ...category };
  }

  closeEditModal(): void {
    this.categoryToEdit = null;
  }

  onCategoryUpdated(updated: Category): void {
    const index = this.categories.findIndex(c => c.id === updated.id);
    if (index !== -1) this.categories[index] = updated;
    this.closeEditModal();
    // TODO: this.toastr.success('Category successfully updated.');
  }

  // ============================================================
  // Archive
  // ============================================================
  openArchiveModal(category: Category): void {
    this.categoryToArchive = category;
  }

  closeArchiveModal(): void {
    this.categoryToArchive = null;
  }

  onArchiveConfirmed(): void {
    if (!this.categoryToArchive) return;
    this.isArchiving = true;
    // TODO:
    // this.categoryService.archiveCategoryAsync(this.categoryToArchive.id)
    //   .then(res => {
    //     if (!res.isSuccess) {
    //       this.toastr.error('Failed to archive category.');
    //       return;
    //     }
    //     this.toastr.success('Category successfully archived.');
    //     this.getAllCategories();
    //   })
    //   .catch(err => console.error(err))
    //   .finally(() => { this.isArchiving = false; this.closeArchiveModal(); });
    this.isArchiving = false;
    this.closeArchiveModal();
  }

  // ============================================================
  // Pagination
  // ============================================================
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.getAllCategories();
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
}