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
  currentPage = 1;
  itemsPerPage = 10;
  isLoading = false;
  employeeDtos: EmployeeDTO[] = [];

  // Deterministic avatar colors based on employee id
  private avatarColors = [
    '#a07840', '#7a9e7e', '#8b7aad', '#4a90a4',
    '#c0697a', '#6b8e6b', '#a0522d', '#5a7a9e',
  ];

  constructor(private employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.getAllEmployees();
  }

  getAllEmployees(): void {
    this.isLoading = true;
    this.employeeService
      .getAllEmployeesAsync(this.currentPage, this.itemsPerPage)
      .then(res => {
        if (!res.isSuccess) return;
        this.employeeDtos = res.data?.items ?? [];
      })
      .catch(err => console.error(err))
      .finally(() => (this.isLoading = false));
  }

  // Returns a consistent color per employee based on their id
  getAvatarColor(id: number): string {
    return this.avatarColors[id % this.avatarColors.length];
  }

  // Fallback: if img fails to load, hide it and show initials avatar
  onImgError(event: Event, emp: EmployeeDTO): void {
    (event.target as HTMLImageElement).style.display = 'none';
    emp.person.profileImagePath = '';
  }
}