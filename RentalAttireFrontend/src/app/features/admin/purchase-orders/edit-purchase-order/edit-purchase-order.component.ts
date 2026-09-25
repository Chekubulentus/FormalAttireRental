import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { SupplierService } from '../../suppliers/suppliers-service/supplier.service';
import { PurchaseOrderService } from '../purchase-order-service/purchase-order.service';
import { ClotheService } from '../../clothes/clothe-service/clothe.service';
import { PurchaseOrderDTO } from '../dtos/purchase-order-dto';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';
import { EditPurchaseOrderCommand } from '../dtos/edit-purchase-order';

// ── Local working types (frontend-only, never sent as-is) ─────────────────────

interface LineItem {
  clothe: ClotheDTO;
  orderedQuantity: number;
  unitCost: number;
  isExisting: boolean;
}

type SubmitMode = 'draft' | 'order';

@Component({
  selector: 'app-edit-purchase-order',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-purchase-order.component.html',
  styleUrl: './edit-purchase-order.component.scss',
})
export class EditPurchaseOrderComponent implements OnInit, OnDestroy {
  private paramMapSub?: Subscription;

  // ── Loaded purchase order context ───────────────────────────────────────
  purchaseOrderId = 0;
  purchaseOrderCode = '';
  supplierId = 0;
  supplierName = '';
  purchaseOrderToBeEdited: PurchaseOrderDTO | null = null;

  isLoading = true;
  loadError = '';

  // ── Clothes for this PO's (fixed) supplier ──────────────────────────────
  availableClothes: ClotheDTO[] = [];
  isClothesLoading = false;
  clotheSearch = '';

  // ── Order details ────────────────────────────────────────────────────────
  expectedDeliveryDate = '';

  // ── Line items (the working cart, pre-populated from the loaded PO) ─────
  lineItems: LineItem[] = [];

  // ── Submission ───────────────────────────────────────────────────────────
  isSubmitting = false;
  touched = false;

  // ── Validation errors ────────────────────────────────────────────────────
  get errors(): Record<string, string> {
    const e: Record<string, string> = {};
    if (!this.expectedDeliveryDate)
      e['expectedDeliveryDate'] = 'Expected delivery date is required.';
    if (this.lineItems.length === 0)
      e['lineItems'] = 'Add at least one item to the order.';
    this.lineItems.forEach((item, i) => {
      if (!item.orderedQuantity || item.orderedQuantity < 1)
        e[`qty_${i}`] = 'Quantity must be at least 1.';
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
    private route: ActivatedRoute,
    private router: Router,
    private supplierService: SupplierService,
    private purchaseOrderService: PurchaseOrderService,
    private clotheService: ClotheService,
    private toastr: ToastrService,
  ) {}

  ngOnInit(): void {
    // Subscribed to paramMap (not route.snapshot read once) — navigating
    // directly from one PO's edit page to another's would otherwise reuse
    // this component instance and silently keep showing the old PO's data.
    this.paramMapSub = this.route.paramMap.subscribe((params) => {
      const idParam = params.get('id');
      const id = idParam ? Number(idParam) : NaN;

      if (!idParam || Number.isNaN(id)) {
        this.toastr.error('Invalid purchase order.', 'Error');
        this.router.navigate(['admin', 'purchase-orders']);
        return;
      }

      this.resetState();
      this.purchaseOrderId = id;
      this.loadPurchaseOrder(id);
    });
  }

  ngOnDestroy(): void {
    this.paramMapSub?.unsubscribe();
  }

  private resetState(): void {
    this.isLoading = true;
    this.loadError = '';
    this.supplierId = 0;
    this.supplierName = '';
    this.availableClothes = [];
    this.clotheSearch = '';
    this.expectedDeliveryDate = '';
    this.lineItems = [];
    this.touched = false;
  }

  // ── Load ──────────────────────────────────────────────────────────────────
  async loadPurchaseOrder(id: number): Promise<void> {
    this.isLoading = true;

    let result;
    try {
      result = await this.purchaseOrderService.getPurchaseOrderByIdAsync(id);
    } catch (err: any) {
      this.toastr.error(err.error);
      this.isLoading = false;
      return;
    }

    if (!result.isSuccess || !result.data) {
      this.toastr.error(
        result.errorMessage ?? 'Purchase order record could not be found.',
      );
      this.isLoading = false;
      return;
    }

    this.purchaseOrderToBeEdited = result.data;

    if (this.purchaseOrderToBeEdited.orderStatus !== 'Draft') {
      this.toastr.error(
        'Only Draft purchase orders can be edited.',
        'Not Editable',
      );
      this.isLoading = false;
      this.router.navigate(['admin', 'purchase-orders']);
      return;
    }

    this.purchaseOrderCode = this.purchaseOrderToBeEdited.purchaseOrderCode;
    this.supplierName = this.purchaseOrderToBeEdited.supplierName;
    this.expectedDeliveryDate = new Date(
      this.purchaseOrderToBeEdited.expectedDeliveryDate,
    )
      .toISOString()
      .split('T')[0];

    const firstItem = this.purchaseOrderToBeEdited.purchaseOrderItems[0];
    if (!firstItem) {
      this.loadError =
        "This draft has no items yet, so its supplier can't be determined.";
      this.toastr.error(this.loadError, 'Error');
      this.isLoading = false;
      return;
    }
    this.supplierId = firstItem.originalSupplierId;
    console.log(`Supplier Identifier: ${firstItem.originalSupplierId}`);

    this.lineItems = this.purchaseOrderToBeEdited.purchaseOrderItems.map(
      (item) => ({
        clothe: item.clothe,
        orderedQuantity: item.orderedQuantity,
        unitCost: item.unitCost,
        isExisting: true,
      }),
    );

    await this.loadClothesForSupplier(this.supplierId);
    this.isLoading = false;
  }

  // ── Clothes for the fixed supplier ──────────────────────────────────────
  async loadClothesForSupplier(supplierId: number): Promise<void> {
    this.isClothesLoading = true;

    this.supplierService
      .getAllSupplierClothesByIdAsync(supplierId)
      .then((res) => {
        if (!res.isSuccess) {
          this.availableClothes = [];
          return;
        }
        this.availableClothes = res.data ?? [];
        console.log(`Supplier Clothes: ${JSON.stringify(res.data)}`);
      })
      .catch((err) => {
        this.toastr.error(err.error);
      })
      .finally(() => {
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
        c.categoryName.toLowerCase().includes(q),
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
          isExisting: false,
        },
      ];
    } else {
      this.lineItems = this.lineItems.filter(
        (li) => li.clothe.id !== clothe.id,
      );
    }
  }

  removeLineItem(clotheId: number): void {
    this.lineItems = this.lineItems.filter((li) => li.clothe.id !== clotheId);
  }

  // ── Totals ─────────────────────────────────────────────────────────────────
  get runningTotal(): number {
    return this.lineItems.reduce(
      (sum, li) => sum + li.orderedQuantity * li.unitCost,
      0,
    );
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-PH', {
      style: 'currency',
      currency: 'PHP',
      minimumFractionDigits: 2,
    }).format(amount);
  }

  // ── Submission ─────────────────────────────────────────────────────────────
  async submit(mode: SubmitMode): Promise<void> {
    this.touched = true;
    if (!this.isValid) return;

    this.isSubmitting = true;

    const command: EditPurchaseOrderCommand = {
      purchaseOrderId: this.purchaseOrderId,
      expectedDeliveryDate: this.expectedDeliveryDate,
      orderStatus: mode === 'draft' ? 'Draft' : 'Ordered',
      lineItems: this.lineItems.map((li) => ({
        clotheId: li.clothe.id,
        quantity: li.orderedQuantity,
        unitCost: li.unitCost,
      })),
    };

    const result =
      await this.purchaseOrderService.updatePurchaseOrderAsync(command);

    if (result.isSuccess) {
      this.toastr.success(
        mode === 'draft'
          ? 'Draft updated.'
          : 'Purchase order placed successfully.',
        mode === 'draft' ? 'Draft Updated' : 'Order Placed',
      );
      this.router.navigate(['admin', 'purchase-orders']);
    } else {
      this.toastr.error(
        result.errorMessage ?? 'Something went wrong.',
        'Error',
      );
    }

    this.isSubmitting = false;
  }

  cancel(): void {
    // Nothing was persisted mid-edit, so Cancel is just a navigate-back —
    // there's nothing to revert.
    this.router.navigate(['admin', 'purchase-orders']);
  }
}
