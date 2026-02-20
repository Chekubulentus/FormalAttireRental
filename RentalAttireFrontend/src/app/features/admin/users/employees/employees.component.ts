import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employees.component.html',
  styleUrl: './employees.component.scss',
})
export class EmployeesComponent {
  searchQuery = '';

  employees = [
    { name: 'Maria Santos',   email: 'maria@elegance.com',   position: 'Store Manager',    department: 'Operations',  dateHired: 'Jan 10, 2021', status: 'Active',   avatarColor: '#a07840' },
    { name: 'Juan dela Cruz', email: 'juan@elegance.com',    position: 'Sales Associate',  department: 'Sales',       dateHired: 'Mar 5, 2022',  status: 'Active',   avatarColor: '#7a9e7e' },
    { name: 'Angela Reyes',   email: 'angela@elegance.com',  position: 'Seamstress',       department: 'Alterations', dateHired: 'Jun 18, 2020', status: 'On Leave', avatarColor: '#8b7aad' },
    { name: 'Carlo Mendoza',  email: 'carlo@elegance.com',   position: 'Inventory Clerk',  department: 'Warehouse',   dateHired: 'Sep 2, 2023',  status: 'Active',   avatarColor: '#4a90a4' },
    { name: 'Diana Lim',      email: 'diana@elegance.com',   position: 'Cashier',          department: 'Finance',     dateHired: 'Feb 14, 2022', status: 'Inactive', avatarColor: '#c0697a' },
    { name: 'Rodel Cruz',     email: 'rodel@elegance.com',   position: 'Delivery Driver',  department: 'Logistics',   dateHired: 'Nov 30, 2021', status: 'Active',   avatarColor: '#6b8e6b' },
  ];
}