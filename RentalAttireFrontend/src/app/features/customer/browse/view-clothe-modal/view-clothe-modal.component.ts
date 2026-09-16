import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CartService } from '../cart.service';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';

@Component({
  selector: 'app-view-clothe-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './view-clothe-modal.component.html',
  styleUrl: './view-clothe-modal.component.scss',
})
export class ViewClotheModalComponent implements OnChanges {

  @Input() clothe!: ClotheDTO;
  @Output() closed = new EventEmitter<void>();
  @Output() addToCart = new EventEmitter<{ clothe: ClotheDTO; quantity: number }>();

  quantity: number = 1;

  constructor(private cartService: CartService) {}

  ngOnChanges(changes: SimpleChanges): void {
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

  get isLowStock(): boolean {
    return !this.isOutOfStock && this.clothe.availableQuantity <= this.clothe.stockQuantity * 0.2;
  }

  // ============================================================
  // Quantity — stepper buttons AND direct typed input
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

  // Runs on every keystroke — deliberately lenient. Only strips decimals,
  // so a value mid-typing (e.g. "1" on its way to "15") isn't fought with
  // clamping while the user is still typing it.
  onQuantityInput(): void {
    if (this.quantity === null || this.quantity === undefined || isNaN(this.quantity)) {
      return;
    }
    this.quantity = Math.floor(this.quantity);
  }

  // Runs when the field loses focus (or Enter is pressed) — this is where
  // the value actually gets clamped into a valid range.
  onQuantityBlur(): void {
    this.quantity = this.sanitizeQuantity(this.quantity);
  }

  private sanitizeQuantity(value: number): number {
    if (value === null || value === undefined || isNaN(value) || value < 1) {
      return this.remainingStock > 0 ? 1 : 0;
    }

    const whole = Math.floor(value);
    return Math.min(whole, this.remainingStock);
  }

  // ============================================================
  // Actions
  // ============================================================
  onAddToCart(): void {
    // Re-sanitize here too, in case Add to Cart is clicked directly
    // (e.g. via Enter) without the input ever losing focus first.
    this.quantity = this.sanitizeQuantity(this.quantity);

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