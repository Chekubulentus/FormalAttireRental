import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ClotheDTO } from '../../../data/models/DTOs/Clothes/clothes';
import { AppToastrService } from '../../../core/services/toastr-service/app-toastr.service';
import { CartItem } from '../../../data/models/DTOs/Clothes/cart-item';

const CART_STORAGE_KEY = 'rental_cart';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItemsSubject = new BehaviorSubject<CartItem[]>(this.loadFromStorage());
  cartItems$ = this.cartItemsSubject.asObservable();

  constructor(private toastr: AppToastrService) {}

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

  addToCart(clothe: ClotheDTO, quantity: number): void {
    const currentItems = this.cartItemsSubject.value;
    const existingIndex = currentItems.findIndex(item => item.clothe.id === clothe.id);

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

    const updatedItems = this.cartItemsSubject.value.map(item =>
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