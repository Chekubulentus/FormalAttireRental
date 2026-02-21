import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../employee-service/employee.service';
import { EmployeeDTO } from '../../../../data/models/DTOs/Employees/employee-dto';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, FormsModule, CurrencyPipe],
  templateUrl: './employees.component.html',
  styleUrl: './employees.component.scss',
})
export class EmployeesComponent implements OnInit {
  searchQuery = '';
  isLoading = false;

  // Pagination state
  currentPage = 1;
  itemsPerPage = 8;
  totalCount = 0;
  totalPages = 1;

  employeeDtos: EmployeeDTO[] = [];

  private avatarColors = [
    '#a07840', '#7a9e7e', '#8b7aad', '#4a90a4',
    '#c0697a', '#6b8e6b', '#a0522d', '#5a7a9e',
  ];

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.getAllEmployees();
  }

  searchEmployees() {
    if(!this.searchQuery.trim())
      this.getAllEmployees();

    this.employeeService.searchEmployeeAsync(this.searchQuery, this.currentPage, this.itemsPerPage)
    .then(res => {
      if(!res.isSuccess)
        return;
      this.employeeDtos = res.data?.items ?? [];
      this.currentPage = 1;
      this.totalCount = res.data?.totalCount ?? 1;
      this.totalPages = res.data?.totalPages ?? 1;
    }).catch(err => {
      console.log(err.error);
    });
  }

  getAllEmployees(): void {
    this.isLoading = true;
    this.employeeService
      .getAllEmployeesAsync(this.currentPage, this.itemsPerPage)
      .then(res => {
        if (!res.isSuccess) return;
        this.employeeDtos  = res.data?.items ?? [];
        this.totalCount    = res.data?.totalCount ?? 0;
        this.totalPages    = res.data?.totalPages ?? 1;
        this.currentPage   = res.data?.pageNumber ?? 1;
      })
      .catch(err => console.error(err))
      .finally(() => (this.isLoading = false));
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.getAllEmployees();
  }

  // Displayed range e.g. "1–10 of 42"
  get rangeStart(): number {
    return Math.min((this.currentPage - 1) * this.itemsPerPage + 1, this.totalCount);
  }

  get rangeEnd(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.totalCount);
  }

  // Builds page number array with ellipsis (-1) where needed
  // e.g. [1, -1, 4, 5, 6, -1, 10]
  get pageNumbers(): number[] {
    const total = this.totalPages;
    const current = this.currentPage;
    const delta = 1; // pages on each side of current
    const pages: number[] = [];

    const range: number[] = [];
    for (let i = Math.max(2, current - delta); i <= Math.min(total - 1, current + delta); i++) {
      range.push(i);
    }

    // Always include first page
    pages.push(1);

    if (range.length > 0 && range[0] > 2) pages.push(-1); // left ellipsis

    pages.push(...range);

    const lastIndex = range.length -1

    if (range.length > 0 && range[lastIndex] < total - 1) pages.push(-1); // right ellipsis

    // Always include last page (if more than 1 page)
    if (total > 1) pages.push(total);

    return pages;
  }

  get filteredEmployees() : EmployeeDTO[] {
    if(!this.searchQuery.trim())
      return this.employeeDtos;

    const query = this.searchQuery.toLowerCase();
    this.currentPage = 1;

    return this.employeeDtos.filter(
      e => e.employeeCode.toLowerCase().includes(query) ||
      e.person.lastName.toLowerCase().includes(query) ||
      e.person.firstName.toLowerCase().includes(query)
    );
  }

  onSearchChange() : EmployeeDTO[]{
    this.currentPage = 1;

    return this.filteredEmployees;
  }

  getAvatarColor(id: number): string {
    return this.avatarColors[id % this.avatarColors.length];
  }

  onImgError(event: Event, emp: EmployeeDTO): void {
    (event.target as HTMLImageElement).style.display = 'none';
    emp.person.profileImagePath = '';
  }
}