import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Category } from '../../../../data/models/DTOs/Category/category';

@Component({
  selector: 'app-edit-category',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-category.component.html',
  styleUrl: './edit-category.component.scss',
})
export class EditCategoryComponent implements OnInit {

  // ============================================================
  // Inputs & Outputs
  // ============================================================

  @Input() category: Category = new Category();

  @Output() closed  = new EventEmitter<void>();
  @Output() updated = new EventEmitter<Category>();


  // ============================================================
  // State
  // ============================================================

  isSaving = false;
  touched  = false;

  // Local copy — edits don't affect the parent list until saved
  form: Category = new Category();

  ngOnInit(): void {
    this.form = { ...this.category };
  }


  // ============================================================
  // Validation
  // ============================================================

  get errors(): Record<string, string> {
    const e: Record<string, string> = {};
    if (!this.form.categoryCode?.trim()) e['categoryCode'] = 'Code is required.';
    if (!this.form.categoryName?.trim()) e['categoryName'] = 'Name is required.';
    return e;
  }

  get isValid(): boolean {
    return Object.keys(this.errors).length === 0;
  }

  hasError(field: string): boolean {
    return this.touched && !!this.errors[field];
  }

  errorMsg(field: string): string {
    return this.touched ? (this.errors[field] ?? '') : '';
  }


  // ============================================================
  // Actions
  // ============================================================

  close(): void {
    this.closed.emit();
  }

  save(): void {
    this.touched = true;
    if (!this.isValid) return;

    this.isSaving = true;
    // TODO: wire CategoryService.updateCategoryAsync(this.form) here
    // this.categoryService.updateCategoryAsync(this.form)
    //   .then(res => {
    //     if (!res.isSuccess) {
    //       this.toastr.error(res.errorMessage ?? 'Failed to update category.');
    //       return;
    //     }
    //     this.toastr.success('Category successfully updated.');
    //     this.updated.emit(this.form);
    //     this.close();
    //   })
    //   .catch(err => console.error(err))
    //   .finally(() => this.isSaving = false);

    // ↓ Remove once API is wired in
    this.updated.emit(this.form);
    this.isSaving = false;
    this.close();
  }
}