import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-accounts',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './accounts.component.html',
  styleUrl: './accounts.component.scss',
})
export class AccountsComponent {
  searchQuery = '';
  filterActive = 'all';

  accounts = [
    { name: 'Maria Santos',   email: 'maria@elegance.com',   role: 'Admin', status: 'Active',    lastLogin: 'Feb 20, 2026', avatarColor: '#a07840' },
    { name: 'Juan dela Cruz', email: 'juan@elegance.com',    role: 'Staff', status: 'Active',    lastLogin: 'Feb 19, 2026', avatarColor: '#7a9e7e' },
    { name: 'Angela Reyes',   email: 'angela@elegance.com',  role: 'Staff', status: 'Inactive',  lastLogin: 'Jan 5, 2026',  avatarColor: '#8b7aad' },
    { name: 'Carlo Mendoza',  email: 'carlo@elegance.com',   role: 'Staff', status: 'Active',    lastLogin: 'Feb 18, 2026', avatarColor: '#4a90a4' },
    { name: 'Diana Lim',      email: 'diana@elegance.com',   role: 'Admin', status: 'Suspended', lastLogin: 'Dec 1, 2025',  avatarColor: '#c0697a' },
    { name: 'Rodel Cruz',     email: 'rodel@elegance.com',   role: 'Staff', status: 'Active',    lastLogin: 'Feb 20, 2026', avatarColor: '#6b8e6b' },
  ];
}