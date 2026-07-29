import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
// import { ClotheService } from '../../../features/clothes/clothe-service/clothe.service';
// import { RentalService } from '../../../features/rentals/rental-service/rental.service';
// import { UserService } from '../../../core/services/user-service/user.service';

// ── Inline models — replace with real imports once created ───
export class ClotheSummary {
  id: number              = 0;
  clotheCode: string      = '';
  clotheName: string      = '';
  categoryName: string    = '';
  clotheGender: string    = '';
  size: string            = '';
  rentalPrice: number     = 0;
  depositAmount: number   = 0;
  condition: string       = '';
  profileImagePath: string = '';
  availableQuantity: number = 0;
  stockQuantity: number   = 0;
}

export class RentalSummary {
  id: number           = 0;
  rentalCode: string   = '';
  status: string       = ''; // 'Pending' | 'Confirmed' | 'Ready for Pickup' | 'Returned'
  startDate: Date | null = null;
  endDate: Date | null   = null;
  totalAmount: number  = 0;
  items: RentalItem[]  = [];
}

export class RentalItem {
  clotheName: string  = '';
  clotheCode: string  = '';
  rentalPrice: number = 0;
}
// ─────────────────────────────────────────────────────────────

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, RouterModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.scss',
})
export class CustomerDashboardComponent implements OnInit {

  // ── State ──────────────────────────────────────────────────
  isLoadingRentals  = false;
  isLoadingClothes  = false;

  customerName      = 'there';
  totalRentals      = 0;
  totalSpent        = 0;
  activeRentals     = 0;

  activeOrders: RentalSummary[]    = [];
  featuredClothes: ClotheSummary[] = [];
  recentRentals: RentalSummary[]   = [];

  // ── Filter for featured clothes ────────────────────────────
  activeCategoryFilter = 'All';
  categoryFilters = ['All', 'Barong', 'Gown', 'Suit', 'Debut', 'Others'];

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loadActiveOrders();
    this.loadFeaturedClothes();
    // TODO: load customer stats from UserService
  }

  loadActiveOrders(): void {
    this.isLoadingRentals = true;

    // TODO: wire to RentalService
    // this.rentalService.getMyActiveRentalsAsync()
    //   .then(res => {
    //     if (!res.isSuccess) return;
    //     this.activeOrders = res.data ?? [];
    //   })
    //   .catch(err => console.error(err))
    //   .finally(() => this.isLoadingRentals = false);

    // Mock data
    this.activeOrders = [
      {
        id: 1, rentalCode: 'RNT-2025-001', status: 'Confirmed',
        startDate: new Date('2025-05-10'), endDate: new Date('2025-05-13'),
        totalAmount: 3500,
        items: [
          { clotheName: 'Ivory Barong Tagalog', clotheCode: 'CLT-001', rentalPrice: 1500 },
          { clotheName: 'Black Slacks',          clotheCode: 'CLT-042', rentalPrice: 500  },
        ]
      },
      {
        id: 2, rentalCode: 'RNT-2025-002', status: 'Ready for Pickup',
        startDate: new Date('2025-05-15'), endDate: new Date('2025-05-18'),
        totalAmount: 4800,
        items: [
          { clotheName: 'Royal Blue Gown', clotheCode: 'CLT-017', rentalPrice: 2500 },
        ]
      },
    ];
    this.activeRentals = this.activeOrders.length;
    this.isLoadingRentals = false;
  }

  loadFeaturedClothes(): void {
    this.isLoadingClothes = true;

    // TODO: wire to ClotheService
    // this.clotheService.filterClotheAsync('', this.activeCategoryFilter === 'All' ? '' : this.activeCategoryFilter, 1, 8)
    //   .then(res => {
    //     if (!res.isSuccess) return;
    //     this.featuredClothes = res.data?.items ?? [];
    //   })
    //   .catch(err => console.error(err))
    //   .finally(() => this.isLoadingClothes = false);

    // Mock data
    this.featuredClothes = [
      { id: 1, clotheCode: 'CLT-001', clotheName: 'Ivory Barong Tagalog',  categoryName: 'Barong', clotheGender: 'Male',   size: 'M',  rentalPrice: 1500, depositAmount: 1500, condition: 'New',  profileImagePath: '', availableQuantity: 5, stockQuantity: 8 },
      { id: 2, clotheCode: 'CLT-002', clotheName: 'Royal Blue Gown',        categoryName: 'Gown',   clotheGender: 'Female', size: 'S',  rentalPrice: 2500, depositAmount: 2500, condition: 'Good', profileImagePath: '', availableQuantity: 2, stockQuantity: 3 },
      { id: 3, clotheCode: 'CLT-003', clotheName: 'Classic Black Suit',     categoryName: 'Suit',   clotheGender: 'Male',   size: 'L',  rentalPrice: 2000, depositAmount: 2000, condition: 'New',  profileImagePath: '', availableQuantity: 4, stockQuantity: 5 },
      { id: 4, clotheCode: 'CLT-004', clotheName: 'Blush Pink Debut Gown',  categoryName: 'Debut',  clotheGender: 'Female', size: 'M',  rentalPrice: 3500, depositAmount: 3500, condition: 'New',  profileImagePath: '', availableQuantity: 1, stockQuantity: 2 },
      { id: 5, clotheCode: 'CLT-005', clotheName: 'Navy Barong Tagalog',    categoryName: 'Barong', clotheGender: 'Male',   size: 'XL', rentalPrice: 1200, depositAmount: 1200, condition: 'Good', profileImagePath: '', availableQuantity: 3, stockQuantity: 4 },
      { id: 6, clotheCode: 'CLT-006', clotheName: 'Champagne Evening Gown', categoryName: 'Gown',   clotheGender: 'Female', size: 'M',  rentalPrice: 2800, depositAmount: 2800, condition: 'New',  profileImagePath: '', availableQuantity: 2, stockQuantity: 3 },
    ];
    this.isLoadingClothes = false;
  }

  setFilter(filter: string): void {
    this.activeCategoryFilter = filter;
    this.loadFeaturedClothes();
  }

  // ── Helpers ────────────────────────────────────────────────
  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending':          'status--pending',
      'Confirmed':        'status--confirmed',
      'Ready for Pickup': 'status--ready',
      'Returned':         'status--returned',
    };
    return map[status] ?? '';
  }

  getStatusStep(status: string): number {
    const steps: Record<string, number> = {
      'Pending': 1, 'Confirmed': 2, 'Ready for Pickup': 3, 'Returned': 4,
    };
    return steps[status] ?? 1;
  }

  getStockStatus(clothe: ClotheSummary): 'available' | 'low' | 'out' {
    if (clothe.availableQuantity === 0) return 'out';
    if (clothe.availableQuantity <= clothe.stockQuantity * 0.2) return 'low';
    return 'available';
  }

  imgErrors: Set<number> = new Set();
  onImgError(id: number): void { this.imgErrors.add(id); }
  hasImgError(id: number): boolean { return this.imgErrors.has(id); }

  get filteredClothes(): ClotheSummary[] {
    if (this.activeCategoryFilter === 'All') return this.featuredClothes;
    return this.featuredClothes.filter(c => c.categoryName === this.activeCategoryFilter);
  }
}