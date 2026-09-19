import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SupplierService } from '../../suppliers/suppliers-service/supplier.service';
import { PurchaseOrderService } from '../purchase-order-service/purchase-order.service';
import { ClotheService } from '../../clothes/clothe-service/clothe.service';
import { SupplierSummaryDTO } from '../../suppliers/dtos/supplier-summary';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';

// ── Local working types (frontend-only, never sent as-is) ─────────────────────

interface LineItem {
  clothe: ClotheDTO;
  orderedQuantity: number;
  unitCost: number;
}

type SubmitMode = 'draft' | 'order';

@Component({
  selector: 'app-create-purchase-order',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-purchase-order.component.html',
  styleUrl: './create-purchase-order.component.scss',
})
export class CreatePurchaseOrderComponent implements OnInit {
  // ── Step tracking ────────────────────────────────────────────────────
  // Step 1: supplier selection
  // Step 2: item selection + order details
  currentStep: 1 | 2 = 1;
  today = new Date().toISOString().split('T')[0];

  // ── Step 1 — Supplier ────────────────────────────────────────────────
  suppliers: SupplierSummaryDTO[] = [];
  supplierSearch = '';
  selectedSupplier: SupplierSummaryDTO | null = null;
  isSuppliersLoading = true;

  // ── Step 2 — Clothes for selected supplier ───────────────────────────
  availableClothes: ClotheDTO[] = [];
  isClothesLoading = false;
  clotheSearch = '';

  // ── Order details ────────────────────────────────────────────────────
  expectedDeliveryDate = '';

  // ── Line items (the working cart) ────────────────────────────────────
  lineItems: LineItem[] = [];

  // ── Submission ───────────────────────────────────────────────────────
  isSubmitting = false;
  touched = false;

  // ── Validation errors ────────────────────────────────────────────────
  get errors(): Record<string, string> {
    const e: Record<string, string> = {};
    if (!this.expectedDeliveryDate) e['expectedDeliveryDate'] = 'Expected delivery date is required.';
    if (this.lineItems.length === 0) e['lineItems'] = 'Add at least one item to the order.';
    this.lineItems.forEach((item, i) => {
      if (!item.orderedQuantity || item.orderedQuantity < 1)
        e[`qty_${i}`] = 'Quantity must be at least 1.';
      if (!item.unitCost || item.unitCost <= 0)
        e[`cost_${i}`] = 'Unit cost must be greater than 0.';
    });
    return e;
  }

  get isValid(): boolean {
    return Object.keys(this.errors).length === 0;
  }

  hasError(field: string): boolean {
    return this.touched && !!this.errors[field];
  }

  errorMsg(field: string): string {
    return this.errors[field] ?? '';
  }

  constructor(
    private supplierService: SupplierService,
    private purchaseOrderService: PurchaseOrderService,
    private clotheService: ClotheService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSuppliers();
  }

  // ── Step 1 — Supplier loading ─────────────────────────────────────────
  async loadSuppliers(): Promise<void> {
    this.isSuppliersLoading = true;

    //FETCH ALL SUPPLIERS
    this.supplierService.getAllSuppliersAsync()
    .then(res => {
      this.suppliers = res.data ?? [];
    }).catch(err => {
      this.toastr.error(err.error);
    }).finally(() => {
      this.isSuppliersLoading = false;
    })

    this.isSuppliersLoading = false;
  }

  get filteredSuppliers(): SupplierSummaryDTO[] {
    const q = this.supplierSearch.toLowerCase();
    if (!q) return this.suppliers;
    return this.suppliers.filter(
      (s) =>
        s.supplierName.toLowerCase().includes(q) ||
        s.supplierCode.toLowerCase().includes(q)
    );
  }

  selectSupplier(supplier: SupplierSummaryDTO): void {
    this.selectedSupplier = supplier;
  }

  async confirmSupplier(): Promise<void> {
    if (!this.selectedSupplier) return;
    this.currentStep = 2;
    await this.loadClothesForSupplier(this.selectedSupplier.id);
  }

  changeSupplier(): void {
    this.selectedSupplier = null;
    this.currentStep = 1;
    this.lineItems = [];
    this.availableClothes = [];
    this.clotheSearch = '';
  }

  // ── Step 2 — Clothes ──────────────────────────────────────────────────
  async loadClothesForSupplier(supplierId: number): Promise<void> {
    this.isClothesLoading = true;

    this.supplierService.getAllSupplierClothesByIdAsync(
      this.selectedSupplier?.id ?? 0
    ).then(res => {
      if(!res.isSuccess) {
        this.availableClothes = [];
        return;
      }
      this.availableClothes = res.data ?? [];
    }).finally(() => {
      this.isClothesLoading = false;
    });
  }

  get filteredClothes(): ClotheDTO[] {
    const q = this.clotheSearch.toLowerCase();
    if (!q) return this.availableClothes;
    return this.availableClothes.filter(
      (c) =>
        c.clotheName.toLowerCase().includes(q) ||
        c.clotheCode.toLowerCase().includes(q) ||
        c.categoryName.toLowerCase().includes(q)
    );
  }

  isSelected(clotheId: number): boolean {
    return this.lineItems.some((li) => li.clothe.id === clotheId);
  }

  toggleClothe(clothe: ClotheDTO): void {
    const idx = this.lineItems.findIndex((li) => li.clothe.id === clothe.id);
    if (idx === -1) {
      this.lineItems = [
        ...this.lineItems,
        {
          clothe,
          orderedQuantity: 1,
          unitCost: clothe.unitCost,
        },
      ];
    } else {
      this.lineItems = this.lineItems.filter((li) => li.clothe.id !== clothe.id);
    }
  }

  removeLineItem(clotheId: number): void {
    this.lineItems = this.lineItems.filter((li) => li.clothe.id !== clotheId);
  }

  // ── Totals ─────────────────────────────────────────────────────────────
  get runningTotal(): number {
    return this.lineItems.reduce(
      (sum, li) => sum + li.orderedQuantity * li.unitCost,
      0
    );
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-PH', {
      style: 'currency',
      currency: 'PHP',
      minimumFractionDigits: 2,
    }).format(amount);
  }

  // ── Submission ─────────────────────────────────────────────────────────
  async submit(mode: SubmitMode): Promise<void> {
    this.touched = true;
    if (!this.isValid) return;
    if (!this.selectedSupplier) return;

    this.isSubmitting = true;

    const payload = {
      supplierId: this.selectedSupplier.id,
      expectedDeliveryDate: this.expectedDeliveryDate,
      targetStatus: mode === 'draft' ? 0 : 1, // 0 = Draft, 1 = Ordered
      items: this.lineItems.map((li) => ({
        clotheId: li.clothe.id,
        orderedQuantity: li.orderedQuantity,
        unitCost: li.unitCost,
      })),
    };

    // TODO: CREATE PURCHASE ORDER — wire up once the backend command exists
    // const result = await this.purchaseOrderService.createPurchaseOrderAsync(payload);
    // if (result.isSuccess) {
    //   this.toastr.success(
    //     mode === 'draft'
    //       ? 'Purchase order saved as draft.'
    //       : 'Purchase order placed successfully.',
    //     mode === 'draft' ? 'Draft Saved' : 'Order Placed'
    //   );
    //   this.router.navigate(['admin', 'purchase-orders']);
    // } else {
    //   this.toastr.error(result.errorMessage ?? 'Something went wrong.', 'Error');
    // }

    this.isSubmitting = false;
  }

  cancel(): void {
    this.router.navigate(['admin', 'purchase-orders']);
  }
}