import { CommonModule, NgIf } from '@angular/common';
import { Component, ElementRef, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmployeeDTO } from '../../../../../data/models/DTOs/Employees/employee-dto';

@Component({
  selector: 'app-add-user',
  standalone: true,
  imports: [FormsModule, NgIf],
  templateUrl: './add-user.component.html',
  styleUrl: './add-user.component.scss',
})
export class AddUserComponent {
  @Output() closed = new EventEmitter<void>();
  @Output() submitted = new EventEmitter<EmployeeDTO>();

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  isSubmitting = false;
  imagePreview: string | null = null;
  selectedFile: File | null = null;

  form: EmployeeDTO = new EmployeeDTO();

  triggerFileInput(): void {
    this.fileInput.nativeElement.click();
  }

  onImageSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.selectedFile = file;
    const reader = new FileReader();
    reader.onload = () => (this.imagePreview = reader.result as string);
    reader.readAsDataURL(file);
  }

  onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.close();
    }
  }

  close(): void {
    this.closed.emit();
  }

  submit(): void {
    if (!this.isValid()) return;
    this.isSubmitting = true;

    // Emit the filled EmployeeDTO up to the parent.
    // Parent will show the account setup popup next.
    // If you need to attach the image file, handle it in the parent via selectedFile.
    this.submitted.emit(this.form);

    this.isSubmitting = false;
  }

  private isValid(): boolean {
    const p = this.form.person;
    return !!(
      p.lastName &&
      p.firstName &&
      p.age &&
      p.gender &&
      p.maritalStatus &&
      p.phoneNumber &&
      p.street &&
      p.baranggay &&
      p.city &&
      p.province &&
      p.postalCode &&
      this.form.employeeCode &&
      this.form.department &&
      this.form.rolePosition &&
      this.form.salary
    );
  }
}
