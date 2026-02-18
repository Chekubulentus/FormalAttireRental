import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss',
})
export class AdminDashboardComponent {
  today = new Date();

  stats = [
    {
      label: 'Total Rentals',
      value: '1,284',
      change: '+12%',
      positive: true,
      iconBg: 'rgba(201, 169, 110, 0.1)',
      borderColor: 'rgba(201, 169, 110, 0.15)',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#c9a96e" stroke-width="1.8">
               <path d="M20.38 3.46L16 2a4 4 0 01-8 0L3.62 3.46a2 2 0 00-1.34 2.23l.58 3.57a1 1 0 00.99.84H6v10c0 1.1.9 2 2 2h8a2 2 0 002-2V10h2.15a1 1 0 00.99-.84l.58-3.57a2 2 0 00-1.34-2.23z"/>
             </svg>`,
    },
    {
      label: 'Active Rentals',
      value: '87',
      change: '+5%',
      positive: true,
      iconBg: 'rgba(111, 207, 151, 0.08)',
      borderColor: 'rgba(111, 207, 151, 0.12)',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#6fcf97" stroke-width="1.8">
               <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/>
             </svg>`,
    },
    {
      label: 'Customers',
      value: '342',
      change: '+8%',
      positive: true,
      iconBg: 'rgba(130, 130, 255, 0.08)',
      borderColor: 'rgba(130, 130, 255, 0.12)',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#8282ff" stroke-width="1.8">
               <path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/>
               <path d="M23 21v-2a4 4 0 00-3-3.87"/><path d="M16 3.13a4 4 0 010 7.75"/>
             </svg>`,
    },
    {
      label: 'Overdue Items',
      value: '9',
      change: '+2',
      positive: false,
      iconBg: 'rgba(229, 115, 115, 0.08)',
      borderColor: 'rgba(229, 115, 115, 0.12)',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#e57373" stroke-width="1.8">
               <circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/>
               <line x1="12" y1="16" x2="12.01" y2="16"/>
             </svg>`,
    },
  ];

  recentRentals = [
    { customer: 'Maria Santos',   item: 'Black Tuxedo (L)',      returnDate: 'Feb 22, 2025', status: 'Active' },
    { customer: 'Juan dela Cruz', item: 'Evening Gown (M)',       returnDate: 'Feb 20, 2025', status: 'Overdue' },
    { customer: 'Angela Reyes',   item: 'Barong Tagalog (XL)',    returnDate: 'Feb 25, 2025', status: 'Active' },
    { customer: 'Carlo Mendoza',  item: 'Blazer Set (S)',         returnDate: 'Feb 18, 2025', status: 'Returned' },
    { customer: 'Diana Lim',      item: 'Cocktail Dress (S)',     returnDate: 'Mar 1, 2025',  status: 'Pending' },
  ];

  inventoryStatus = [
    { label: 'Available', count: 148, percent: 72, color: '#6fcf97' },
    { label: 'Rented',    count: 87,  percent: 42, color: '#c9a96e' },
    { label: 'Overdue',   count: 9,   percent: 4,  color: '#e57373' },
    { label: 'Repair',    count: 12,  percent: 6,  color: '#8282ff' },
  ];

  upcomingReturns = [
    { customer: 'Maria Santos',   item: 'Black Tuxedo (L)',   date: 'Feb 22', overdue: false },
    { customer: 'Juan dela Cruz', item: 'Evening Gown (M)',   date: 'Feb 20', overdue: true  },
    { customer: 'Angela Reyes',   item: 'Barong Tagalog (XL)', date: 'Feb 25', overdue: false },
  ];
}