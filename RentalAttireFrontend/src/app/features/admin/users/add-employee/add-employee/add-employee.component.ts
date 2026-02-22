import { CommonModule } from '@angular/common';
import { Component, ElementRef, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmployeeDTO } from '../../../../../data/models/DTOs/Employees/employee-dto';

@Component({
  selector: 'app-add-employee',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-employee.component.html',
  styleUrl: './add-employee.component.scss',
})
export class AddEmployeeComponent {
  @Output() closed    = new EventEmitter<void>();
  @Output() submitted = new EventEmitter<EmployeeDTO>();

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  isSubmitting = false;
  imagePreview: string | null = null;
  selectedFile: File | null   = null;

  form: EmployeeDTO = {
    id: 0,
    employeeCode: '',
    department: '',
    salary: null as any,
    rolePosition: '',
    person: {
      id: 0,
      lastName: '',
      firstName: '',
      middleName: '',
      age: null as any,
      gender: '',
      maritalStatus: '',
      phoneNumber: '',
      street: '',
      baranggay: '',
      city: '',
      province: '',
      postalCode: '',
      profileImagePath: '',
      fullName: '',
    },
  };

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

  close(): void {
    this.closed.emit();
  }

  submit(): void {
    if (!this.isValid()) return;
    this.isSubmitting = true;
    this.submitted.emit(this.form);
    this.isSubmitting = false;
  }

  private isValid(): boolean {
    const p = this.form.person;
    return !!(
      p.lastName && p.firstName && p.age && p.gender &&
      p.maritalStatus && p.phoneNumber && p.street &&
      p.baranggay && p.city && p.province && p.postalCode &&
      this.form.employeeCode && this.form.department &&
      this.form.rolePosition && this.form.salary
    );
  }
}