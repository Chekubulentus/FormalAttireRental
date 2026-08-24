import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { ClotheDTO } from '../../../data/models/DTOs/Clothes/clothes';
import { CartItem } from '../../../data/models/DTOs/Clothes/cart-item';

const CART_STORAGE_KEY = 'rental_cart';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItemsSubject = new BehaviorSubject<CartItem[]>(this.loadFromStorage());
  cartItems$ = this.cartItemsSubject.asObservable();

  constructor(private toastr: ToastrService) {}

  private loadFromStorage(): CartItem[] {
    try {
      const raw = localStorage.getItem(CART_STORAGE_KEY);
      return raw ? (JSON.parse(raw) as CartItem[]) : [];
    } catch {
      return [];
    }
  }

  private saveToStorage(items: CartItem[]): void {
    localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(items));
  }

  private updateCart(items: CartItem[]): void {
    this.saveToStorage(items);
    this.cartItemsSubject.next(items);
  }

  getCartItems(): CartItem[] {
    return this.cartItemsSubject.value;
  }

  getItemCount(): number {
    return this.cartItemsSubject.value.reduce((sum, item) => sum + item.quantity, 0);
  }

  isInCart(clotheId: number): boolean {
    return this.cartItemsSubject.value.some(item => item.clothe.id === clotheId);
  }

  getQuantityInCart(clotheId: number): number {
    const existing = this.cartItemsSubject.value.find(item => item.clothe.id === clotheId);
    return existing ? existing.quantity : 0;
  }

  addToCart(clothe: ClotheDTO, quantity: number): void {
    const currentItems = this.cartItemsSubject.value;
    const existingIndex = currentItems.findIndex(item => item.clothe.id === clothe.id);
    const existingQuantity = existingIndex > -1 ? currentItems[existingIndex].quantity : 0;

    // Stock guard: reject entirely if this request would push the item's
    // cart quantity past what's actually available.
    if (existingQuantity + quantity > clothe.availableQuantity) {
      const remaining = clothe.availableQuantity - existingQuantity;

      if (remaining <= 0) {
        this.toastr.warning(`${clothe.clotheName} is already at the maximum available quantity in your cart.`);
      } else {
        this.toastr.warning(`Only ${remaining} more ${clothe.clotheName} available.`);
      }
      return;
    }

    let updatedItems: CartItem[];

    if (existingIndex > -1) {
      updatedItems = currentItems.map((item, index) =>
        index === existingIndex
          ? { ...item, quantity: item.quantity + quantity }
          : item
      );
    } else {
      updatedItems = [...currentItems, { clothe, quantity }];
    }

    this.updateCart(updatedItems);
    this.toastr.success(`${clothe.clotheName} added to cart.`);
  }

  updateQuantity(clotheId: number, quantity: number): void {
    if (quantity <= 0) {
      this.removeFromCart(clotheId);
      return;
    }

    const currentItems = this.cartItemsSubject.value;
    const existing = currentItems.find(item => item.clothe.id === clotheId);

    if (existing && quantity > existing.clothe.availableQuantity) {
      this.toastr.warning(`Only ${existing.clothe.availableQuantity} ${existing.clothe.clotheName} available.`);
      return;
    }

    const updatedItems = currentItems.map(item =>
      item.clothe.id === clotheId ? { ...item, quantity } : item
    );

    this.updateCart(updatedItems);
  }

  removeFromCart(clotheId: number): void {
    const updatedItems = this.cartItemsSubject.value.filter(item => item.clothe.id !== clotheId);
    this.updateCart(updatedItems);
  }

  clearCart(): void {
    this.updateCart([]);
  }
}