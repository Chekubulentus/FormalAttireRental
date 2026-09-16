import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RentalDTO } from '../../../../data/models/DTOs/Rentals/rental';
import { RentalItemDTO } from '../../../../data/models/DTOs/Rentals/rental-item';

@Component({
  selector: 'app-view-rental',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './view-rental.component.html',
  styleUrl: './view-rental.component.scss',
})
export class ViewRentalComponent {

  @Input() isOpen: boolean = false;
  @Input() rental: RentalDTO | null = null;

  @Output() closed = new EventEmitter<void>();

  // ── Progress tracker steps — same labels/order as MyRentalsComponent ──
  readonly statusSteps = ['Pending', 'Confirmed', 'Ready for Pickup', 'Returned'];
  private readonly statusOrder = ['pending', 'confirmed', 'ready for pickup', 'returned'];

  onOverlayClick(): void {
    this.closed.emit();
  }

  // ============================================================
  // Status matching — duplicated from MyRentalsComponent rather
  // than shared, since there's no existing shared status-utility
  // service/pipe in the codebase yet. Flag me if you'd rather I
  // extract this into one and refactor both components.
  // ============================================================
  private normalizeStatus(status: string | undefined): string {
    return (status ?? '').trim().toLowerCase();
  }

  isDeclined(): boolean {
    return this.normalizeStatus(this.rental?.status) === 'declined';
  }

  isPending(): boolean {
    return this.normalizeStatus(this.rental?.status) === 'pending';
  }

  getStatusStep(): number {
    const idx = this.statusOrder.indexOf(this.normalizeStatus(this.rental?.status));
    return idx === -1 ? 0 : idx + 1;
  }

  badgeClass(): string {
    switch (this.normalizeStatus(this.rental?.status)) {
      case 'pending':           return 'status-badge--pending';
      case 'confirmed':         return 'status-badge--confirmed';
      case 'ready for pickup':  return 'status-badge--ready';
      case 'returned':          return 'status-badge--returned';
      case 'declined':          return 'status-badge--declined';
      default:                  return '';
    }
  }

  // ============================================================
  // Payment display
  // ============================================================
  isGcash(): boolean {
    return this.rental?.paymentMethod === 'Gcash';
  }

  // ============================================================
  // Items
  // ============================================================
  get items(): RentalItemDTO[] {
    return this.rental?.rentalItems ?? [];
  }

  trackByItem(index: number, item: RentalItemDTO): number {
    return item.id;
  }
}