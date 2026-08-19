import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';
import { CartService } from '../cart.service';

@Component({
  selector: 'app-view-clothe-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './view-clothe-modal.component.html',
  styleUrl: './view-clothe-modal.component.scss',
})
export class ViewClotheModalComponent implements OnChanges {

  @Input() clothe!: ClotheDTO;
  @Output() closed = new EventEmitter<void>();
  @Output() addToCart = new EventEmitter<{ clothe: ClotheDTO; quantity: number }>();

  quantity = 1;

  constructor(private cartService: CartService) {}

  ngOnChanges(changes: SimpleChanges): void {
    // Reset quantity back to 1 (or lower, if remaining stock is less) every time a new clothe is viewed
    if (changes['clothe']) {
      this.quantity = this.remainingStock > 0 ? 1 : 0;
    }
  }

  // ============================================================
  // Stock Awareness
  // ============================================================
  get alreadyInCart(): number {
    return this.cartService.getQuantityInCart(this.clothe.id);
  }

  get remainingStock(): number {
    return Math.max(0, this.clothe.availableQuantity - this.alreadyInCart);
  }

  get isOutOfStock(): boolean {
    return this.remainingStock <= 0;
  }

  // ============================================================
  // Quantity Stepper
  // ============================================================
  increaseQuantity(): void {
    if (this.quantity < this.remainingStock) {
      this.quantity++;
    }
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  // ============================================================
  // Actions
  // ============================================================
  onAddToCart(): void {
    if (this.isOutOfStock || this.quantity < 1) return;

    this.addToCart.emit({ clothe: this.clothe, quantity: this.quantity });
  }

  onClose(): void {
    this.closed.emit();
  }

  onOverlayClick(): void {
    this.onClose();
  }

  // ============================================================
  // Helpers
  // ============================================================
  onImgError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }
}