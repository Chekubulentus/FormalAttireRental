import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { CartService } from '../cart.service';
import { CartItem } from '../../../../data/models/DTOs/Clothes/cart-item';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent implements OnInit, OnDestroy {

  @Input() isOpen = false;
  @Output() closed = new EventEmitter<void>();

  items: CartItem[] = [];
  private sub?: Subscription;

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.sub = this.cartService.cartItems$.subscribe(items => {
      this.items = items;
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
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

  get isEmpty(): boolean {
    return this.items.length === 0;
  }

  // ============================================================
  // Quantity Controls
  // ============================================================
  increaseQuantity(item: CartItem): void {
    if (item.quantity < item.clothe.availableQuantity) {
      this.cartService.updateQuantity(item.clothe.id, item.quantity + 1);
    }
  }

  decreaseQuantity(item: CartItem): void {
    if (item.quantity > 1) {
      this.cartService.updateQuantity(item.clothe.id, item.quantity - 1);
    } else {
      this.removeItem(item);
    }
  }

  removeItem(item: CartItem): void {
    this.cartService.removeFromCart(item.clothe.id);
  }

  atMaxQuantity(item: CartItem): boolean {
    return item.quantity >= item.clothe.availableQuantity;
  }

  // ============================================================
  // Actions
  // ============================================================
  onClose(): void {
    this.closed.emit();
  }

  onOverlayClick(): void {
    this.onClose();
  }

  onImgError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }
}