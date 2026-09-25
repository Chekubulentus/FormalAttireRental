import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PurchaseOrderService } from '../purchase-order-service/purchase-order.service';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';
import { PurchaseOrderDTO } from '../dtos/purchase-order-dto';
import { ViewPurchaseOrderComponent } from '../view-purchase-order/view-purchase-order.component';

type StatusFilter = 'Draft' | 'Ordered' | 'PartiallyReceived' | 'Received' | 'Cancelled';

// ASSUMPTION: low-stock threshold — not confirmed against any business rule, adjust freely
const LOW_STOCK_THRESHOLD = 5;

@Component({
  selector: 'app-purchase-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ViewPurchaseOrderComponent],
  templateUrl: './purchase-orders.component.html',
  styleUrl: './purchase-orders.component.scss'
})
export class PurchaseOrdersComponent implements OnInit {

  // ============================================================
  // Data
  // ============================================================
  purchaseOrders: PurchaseOrderDTO[] = [];
  isLoading = true;

  // ============================================================
  // Low stock / out of stock panel
  // PLACEHOLDER DATA — no backend query for this exists yet.
  // Replace loadStockAlerts() with a real service call once
  // a GetLowStockClothes-style endpoint is built.
  // ============================================================
  stockAlertClothes: ClotheDTO[] = [];
  isLoadingStockAlerts = true;

  // ============================================================
  // Filters
  // ============================================================
  searchQuery = '';
  availableStatuses: StatusFilter[] = ['Draft', 'Ordered', 'PartiallyReceived', 'Received', 'Cancelled'];
  selectedStatuses: StatusFilter[] = [];
  dateTypeToggle: 'OrderDate' | 'ExpectedDeliveryDate' = 'OrderDate';
  startingDate: string | null = null;
  endingDate: string | null = null;

  // ============================================================
  // Pagination
  // ============================================================
  currentPage = 1;
  itemsPerPage = 10;
  totalCount = 0;
  totalPages = 0;

  // ============================================================
  // View modal
  // ============================================================
  isViewModalOpen = false;
  selectedPurchaseOrderId: number | null = null;

  get rangeStart(): number {
    return this.totalCount === 0 ? 0 : (this.currentPage - 1) * this.itemsPerPage + 1;
  }

  get rangeEnd(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.totalCount);
  }

  get pageNumbers(): number[] {
    const pages: number[] = [];
    for (let i = 1; i <= this.totalPages; i++) {
      if (i === 1 || i === this.totalPages || Math.abs(i - this.currentPage) <= 1) {
        pages.push(i);
      } else if (pages[pages.length - 1] !== -1) {
        pages.push(-1);
      }
    }
    return pages;
  }

  constructor(
    private purchaseOrderService: PurchaseOrderService,
    private toastrService: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPurchaseOrders();
    this.loadStockAlerts();
  }

  // ============================================================
  // Load
  // ============================================================
  async loadPurchaseOrders(): Promise<void> {
    this.isLoading = true;

    const res = await this.purchaseOrderService.filterPurchaseOrdersAsync(
      this.searchQuery,
      this.selectedStatuses,
      this.dateTypeToggle,
      this.startingDate ? new Date(this.startingDate) : null,
      this.endingDate ? new Date(this.endingDate) : null,
      this.currentPage,
      this.itemsPerPage
    );

    if (!res.isSuccess) {
      this.toastrService.error(res.errorMessage ?? 'Failed to load purchase orders.');
      this.purchaseOrders = [];
      this.isLoading = false;
      return;
    }

    this.purchaseOrders = res.data?.items ?? [];
    this.totalCount = res.data?.totalCount ?? 0;
    this.totalPages = res.data?.totalPages ?? 1;
    this.isLoading = false;
  }

  // PLACEHOLDER — mocked until a real "low stock" backend query exists.
  async loadStockAlerts(): Promise<void> {
    this.isLoadingStockAlerts = true;

    // TODO: replace with e.g. this.clotheService.getStockAlertsAsync()
    await new Promise(r => setTimeout(r, 300));

    this.stockAlertClothes = [];
    this.isLoadingStockAlerts = false;
  }

  isOutOfStock(clothe: ClotheDTO): boolean {
    return clothe.availableQuantity === 0;
  }

  isLowStock(clothe: ClotheDTO): boolean {
    return clothe.availableQuantity > 0 && clothe.availableQuantity <= LOW_STOCK_THRESHOLD;
  }

  // ============================================================
  // Filters
  // ============================================================
  toggleStatus(status: StatusFilter): void {
    const idx = this.selectedStatuses.indexOf(status);
    if (idx > -1) {
      this.selectedStatuses.splice(idx, 1);
    } else {
      this.selectedStatuses.push(status);
    }
    this.currentPage = 1;
    this.loadPurchaseOrders();
  }

  isStatusSelected(status: StatusFilter): boolean {
    return this.selectedStatuses.includes(status);
  }

  onSearchChange(): void {
    this.currentPage = 1;
    this.loadPurchaseOrders();
  }

  onDateFilterChange(): void {
    this.currentPage = 1;
    this.loadPurchaseOrders();
  }

  // ============================================================
  // Pagination
  // ============================================================
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.loadPurchaseOrders();
  }

  // ============================================================
  // Row actions
  // ============================================================

  editPurchaseOrder(po: PurchaseOrderDTO): void {
    this.router.navigate(['/admin/purchase-orders', po.id, 'edit']);
  }

  receivePurchaseOrder(po: PurchaseOrderDTO): void {
    this.router.navigate(['/admin/purchase-orders', po.id, 'receive']);
  }

  canEdit(po: PurchaseOrderDTO): boolean {
    return po.orderStatus === 'Draft';
  }

  canReceive(po: PurchaseOrderDTO): boolean {
    return po.orderStatus === 'Ordered' || po.orderStatus === 'PartiallyReceived';
  }

  // ============================================================
  // Status badge styling
  // ============================================================
  statusClass(status: string): string {
    switch (status) {
      case 'Draft': return 'status-draft';
      case 'Ordered': return 'status-ordered';
      case 'PartiallyReceived': return 'status-partial';
      case 'Received': return 'status-received';
      case 'Cancelled': return 'status-cancelled';
      default: return '';
    }
  }

  navigateToCreate(): void {
    this.router.navigate(['/admin/create-purchase-order']);
  }

  viewPurchaseOrder(po: PurchaseOrderDTO): void {
    this.selectedPurchaseOrderId = po.id;
    this.isViewModalOpen = true;
  }

  onViewModalClosed(): void {
    this.isViewModalOpen = false;
    this.selectedPurchaseOrderId = null;
  }
}