import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { RentalsService } from '../rentals.service';
import { RentalDTO } from '../../../../data/models/DTOs/Rentals/rental';
import { RentalStatus } from '../../../../data/models/DTOs/Rentals/rental-status';

@Component({
  selector: 'app-view-rental',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './view-rental.component.html',
  styleUrl: './view-rental.component.scss',
})
export class ViewRentalComponent {

  @Input() rental!: RentalDTO;
  @Output() closed = new EventEmitter<void>();
  @Output() statusUpdated = new EventEmitter<void>();

  isUpdating = false;

  constructor(
    private rentalService: RentalsService,
    private toastr: ToastrService
  ) {}

  // ============================================================
  // Close
  // ============================================================
  close(): void {
    if (this.isUpdating) return;
    this.closed.emit();
  }

  // ============================================================
  // Confirm / Decline
  // ============================================================
  confirmRental(): void {
    this.updateStatus('Confirmed');
  }

  declineRental(): void {
    this.updateStatus('Declined');
  }

  private updateStatus(status: RentalStatus): void {
    if (this.isUpdating) return;
    this.isUpdating = true;

    this.rentalService.updateRentalStatus(this.rental.id, status)
      .then(res => {
        if (!res.isSuccess) {
          this.toastr.error(res.errorMessage ?? 'Failed to update rental status.');
          return;
        }
        this.toastr.success(res.successMessage ?? `Rental ${status.toLowerCase()} successfully.`);
        this.statusUpdated.emit();
        this.closed.emit();
      })
      .catch(err => {
        this.toastr.error(err.error ?? 'Something went wrong.');
      })
      .finally(() => {
        this.isUpdating = false;
      });
  }

  // ============================================================
  // Helpers
  // ============================================================
  get isPending(): boolean {
    return this.rental.status === 'Pending';
  }

  get customerInitials(): string {
    const first = this.rental.customer.person.firstName?.charAt(0) ?? '';
    const last = this.rental.customer.person.lastName?.charAt(0) ?? '';
    return (first + last).toUpperCase() || '—';
  }

  get displayRentalCode(): string {
    return this.rental.rentalCode?.trim() ? this.rental.rentalCode : `RNT-${this.rental.id.toString().padStart(6, '0')}`;
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Pending':          return 'status-pill--pending';
      case 'Confirmed':        return 'status-pill--confirmed';
      case 'Ready for pickup': return 'status-pill--ready';
      case 'Returned':         return 'status-pill--returned';
      case 'Declined':         return 'status-pill--declined';
      default:                 return '';
    }
  }

  onImgError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }
}