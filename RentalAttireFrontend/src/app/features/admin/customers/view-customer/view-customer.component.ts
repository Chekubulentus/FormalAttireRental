import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Customer } from '../../../../data/models/DTOs/Customer/customer';
// Once you have real imports replace the above with:
// import { CustomerDTO } from '../../../../data/models/DTOs/Customers/customer-dto';

@Component({
  selector: 'app-view-customer',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './view-customer.component.html',
  styleUrl: './view-customer.component.scss',
})
export class ViewCustomerComponent {

  @Input() customer: Customer = new Customer();
  @Output() closed = new EventEmitter<void>();

  imgError = false;

  close(): void {
    this.closed.emit();
  }

  onImgError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
    this.imgError = true;
  }

  // ── Helpers ────────────────────────────────────────────────

  getInitials(fullName: string): string {
    const parts = fullName?.trim().split(' ') ?? [];
    if (parts.length === 0) return '?';
    if (parts.length === 1) return parts[0][0]?.toUpperCase() ?? '?';
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }

  private avatarColors = [
    '#a07840', '#7a9e7e', '#8b7aad', '#4a90a4',
    '#c0697a', '#6b8e6b', '#a0522d', '#5a7a9e',
  ];

  getAvatarColor(id: number): string {
    return this.avatarColors[id % this.avatarColors.length];
  }

  get avgSpentPerRental(): number {
    if (!this.customer.totalRentals) return 0;
    return this.customer.totalSpent / this.customer.totalRentals;
  }
}