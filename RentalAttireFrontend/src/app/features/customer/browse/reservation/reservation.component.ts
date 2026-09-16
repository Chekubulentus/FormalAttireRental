import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CartService } from '../cart.service';
import { CartItem } from '../../../../data/models/DTOs/Clothes/cart-item';
import { RentalTransactionRequest } from '../../../../data/models/DTOs/Rentals/rental-transaction-request';
import { ReservationService } from '../reservation-service/reservation.service';
import { MessageModalComponent } from '../../../../shared/components/message-modal/message-modal.component';

@Component({
  selector: 'app-reservation',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    RouterModule,
    MessageModalComponent
  ],
  templateUrl: './reservation.component.html',
  styleUrl: './reservation.component.scss',
})
export class ReservationComponent implements OnInit {

  items: CartItem[] = [];
  isSubmitting = false;
  touched = false;
  showMessageModal : boolean = false;
  messageModalTitle : string = '';
  messageModalMessage: string = '';

  form = {
    pickupDate: '',
    returnDate: '',
    gcashRefNum: '',
    gcashRefName: '',
  };

  minPickupDate = this.toDateInputValue(new Date());

  constructor(
    private cartService: CartService,
    private rentalService: ReservationService,
    private toastr: ToastrService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.items = this.cartService.getCartItems();

    // Guard: reservation only makes sense with items in the cart.
    // Covers direct URL nav, refresh, or browser back after checkout.
    if (this.items.length === 0) {
      this.router.navigateByUrl('/customer/browse');
    }
  }

  // ============================================================
  // Totals
  // ============================================================
  get subtotal(): number {
    return this.items.reduce((sum, item) => sum + item.clothe.rentalPrice * item.quantity, 0);
  }

  get depositTotal(): number {
    return this.items.reduce((sum, item) => sum + item.clothe.depositAmount * item.quantity, 0);
  }

  get grandTotal(): number {
    return this.subtotal + this.depositTotal;
  }

  // ============================================================
  // Validation
  // ============================================================
  get errors(): Record<string, string> {
    const e: Record<string, string> = {};

    if (!this.form.pickupDate) e['pickupDate'] = 'Pickup date is required.';
    if (!this.form.returnDate) e['returnDate'] = 'Return date is required.';

    if (this.form.pickupDate && this.form.pickupDate < this.minPickupDate) {
      e['pickupDate'] = 'Pickup date cannot be in the past.';
    }

    if (this.form.pickupDate && this.form.returnDate && this.form.returnDate <= this.form.pickupDate) {
      e['returnDate'] = 'Return date must be after the pickup date.';
    }

    if (!this.form.gcashRefNum?.trim()) e['gcashRefNum'] = 'GCash reference number is required.';
    if (!this.form.gcashRefName?.trim()) e['gcashRefName'] = 'Name on the GCash transaction is required.';

    return e;
  }

  get isValid(): boolean {
    return Object.keys(this.errors).length === 0;
  }

  hasError(field: string): boolean {
    return this.touched && !!this.errors[field];
  }

  errorMsg(field: string): string {
    return this.touched ? (this.errors[field] ?? '') : '';
  }

  // ============================================================
  // Submit
  // ============================================================
  async onSubmit(): Promise<void> {
    this.touched = true;
    if (!this.isValid || this.isSubmitting) return;

    this.isSubmitting = true;

    const request: RentalTransactionRequest = {
      pickupDate: new Date(this.form.pickupDate),
      returnDate: new Date(this.form.returnDate),
      paymentMethod: 'Gcash',
      gcashRefNum: this.form.gcashRefNum.trim(),
      gcashRefName: this.form.gcashRefName.trim(),
      rentalItems: this.items.map(item => ({
        clotheId: item.clothe.id,
        quantity: item.quantity,
      })),
    };

    try {
      const res = await this.rentalService.rentalReservationAsync(request);

      if (!res.isSuccess) {
        this.toastr.error(res.errorMessage ?? 'Something went wrong. Please try again.');
        return;
      }

      this.cartService.clearCart();

      this.showMessageModal = true;
      this.messageModalMessage = res.successMessage ?? "";
      this.messageModalTitle = 'Rental Reservation Submitted';


    } catch {
      this.toastr.error('Something went wrong. Please try again.');
    } finally {
      this.isSubmitting = false;
    }
  }

  messageModalConfirmation() {
    this.showMessageModal = false;
    this.router.navigateByUrl('/customer/customer-dashboard');
  }

  // ============================================================
  // Helpers
  // ============================================================
  private toDateInputValue(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  onImgError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }
}