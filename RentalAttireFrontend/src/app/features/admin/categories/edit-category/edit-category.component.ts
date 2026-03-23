import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Category } from '../../../../data/models/DTOs/Category/category';
import { CategoryService } from '../category-service/category.service';
import { UpdateCategoryCommand } from '../../../../data/models/DTOs/Category/update-category';
import { UserViewModel } from '../../../../data/models/DTOs/Users/user-view-model';
import { ToastrService } from 'ngx-toastr';

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
  @Input() currentUser : UserViewModel | undefined;

  @Output() closed  = new EventEmitter<void>();
  @Output() updated = new EventEmitter<Category>();


  // ============================================================
  // State
  // ============================================================

  isSaving = false;
  touched  = false;

  // Local copy — edits don't affect the parent list until saved
  form: Category = new Category();

  constructor(
    private categoryService : CategoryService,
    private toastrService : ToastrService
  ) {}

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

    const payload : UpdateCategoryCommand = {
      id : this.form.id,
      categoryCode : this.form.categoryCode,
      categoryName : this.form.categoryName,
      description : this.form.description,
      performedBy : this.currentUser?.fullName ?? '',
      performedById : this.currentUser?.id ?? 0
    };

    this.categoryService.updateCategoryAsync(
      payload
    ).then(res => {
      if(!res.isSuccess) 
        this.toastrService.error(res.errorMessage);
      this.updated.emit(this.form);
    }).catch(err => {
      this.toastrService.error(err.error);
      this.close();
    }).finally(() => {
      this.isSaving = false;
    });
  }
}