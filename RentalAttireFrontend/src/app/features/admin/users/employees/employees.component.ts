import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../employee-service/employee.service';
import { EmployeeDTO } from '../../../../data/models/DTOs/Employees/employee-dto';
import { AddEmployeeComponent } from '../add-employee/add-employee/add-employee.component';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, FormsModule, CurrencyPipe, AddEmployeeComponent],
  templateUrl: './employees.component.html',
  styleUrl: './employees.component.scss',
})
export class EmployeesComponent implements OnInit {
  searchQuery  = '';
  isLoading    = false;
  showModal    = false;

  pendingEmployee: EmployeeDTO | null = null;
  showAccountModal = false;

  currentPage  = 1;
  itemsPerPage = 8;
  totalCount   = 0;
  totalPages   = 1;

  employeeDtos: EmployeeDTO[] = [];

  private avatarColors = [
    '#a07840', '#7a9e7e', '#8b7aad', '#4a90a4',
    '#c0697a', '#6b8e6b', '#a0522d', '#5a7a9e',
  ];

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.getAllEmployees();
  }

  searchEmployees(): void {
    if (!this.searchQuery.trim()) {
      this.currentPage = 1;
      this.getAllEmployees();
      return;
    }
    this.isLoading = true;
    this.currentPage = 1;
    this.employeeService
      .searchEmployeeAsync(this.searchQuery, this.currentPage, this.itemsPerPage)
      .then(res => {
        if (!res.isSuccess) { this.employeeDtos = []; this.totalCount = 0; this.totalPages = 1; return; }
        this.employeeDtos = res.data?.items ?? [];
        this.totalCount   = res.data?.totalCount ?? 0;
        this.totalPages   = res.data?.totalPages ?? 1;
      })
      .catch(err => console.error(err))
      .finally(() => (this.isLoading = false));
  }

  getAllEmployees(): void {
    this.isLoading = true;
    this.employeeService
      .getAllEmployeesAsync(this.currentPage, this.itemsPerPage)
      .then(res => {
        if (!res.isSuccess) return;
        this.employeeDtos = res.data?.items ?? [];
        this.totalCount   = res.data?.totalCount ?? 0;
        this.totalPages   = res.data?.totalPages ?? 1;
        this.currentPage  = res.data?.pageNumber ?? 1;
      })
      .catch(err => console.error(err))
      .finally(() => (this.isLoading = false));
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    if (!this.searchQuery.trim()) {
      this.getAllEmployees();
    } else {
      this.isLoading = true;
      this.employeeService
        .searchEmployeeAsync(this.searchQuery, this.currentPage, this.itemsPerPage)
        .then(res => {
          if (!res.isSuccess) return;
          this.employeeDtos = res.data?.items ?? [];
          this.totalCount   = res.data?.totalCount ?? 0;
          this.totalPages   = res.data?.totalPages ?? 1;
        })
        .catch(err => console.error(err))
        .finally(() => (this.isLoading = false));
    }
  }

  onOverlayClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.showModal = false;
    }
  }

  onEmployeeSubmitted(employee: EmployeeDTO): void {
    this.pendingEmployee = employee;
    this.showModal = false;
    this.showAccountModal = true;
  }

  get rangeStart(): number {
    return Math.min((this.currentPage - 1) * this.itemsPerPage + 1, this.totalCount);
  }

  get rangeEnd(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.totalCount);
  }

  get pageNumbers(): number[] {
    const total = this.totalPages;
    const current = this.currentPage;
    const delta = 1;
    const pages: number[] = [];
    const range: number[] = [];

    for (let i = Math.max(2, current - delta); i <= Math.min(total - 1, current + delta); i++) {
      range.push(i);
    }

    pages.push(1);
    if (range.length > 0 && range[0] > 2) pages.push(-1);
    pages.push(...range);
    if (range.length > 0 && range[range.length - 1] < total - 1) pages.push(-1);
    if (total > 1) pages.push(total);

    return pages;
  }

  getAvatarColor(id: number): string {
    return this.avatarColors[id % this.avatarColors.length];
  }

  onImgError(event: Event, emp: EmployeeDTO): void {
    (event.target as HTMLImageElement).style.display = 'none';
    emp.person.profileImagePath = '';
  }
}