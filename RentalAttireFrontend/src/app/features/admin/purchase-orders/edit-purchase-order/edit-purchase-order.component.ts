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

// ── Local working types (frontend-only, never sent as-is) ─────────────────────

interface LineItem {
  clothe: ClotheDTO;
  orderedQuantity: number;
  unitCost: number;
  // UI-only bookkeeping — not sent to the backend. Distinguishes a line that
  // already existed on the PO from one added during this edit session, in
  // case we ever want to style them differently.
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
    if (!this.expectedDeliveryDate) e['expectedDeliveryDate'] = 'Expected delivery date is required.';
    if (this.lineItems.length === 0) e['lineItems'] = 'Add at least one item to the order.';
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
    private toastr: ToastrService
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

    // TODO: GET PURCHASE ORDER BY ID — wire up once the backend query exists
    // const result = await this.purchaseOrderService.getPurchaseOrderByIdAsync(id);
    // if (!result.isSuccess || !result.data) {
    //   this.loadError = result.errorMessage ?? 'Purchase order not found.';
    //   this.toastr.error(this.loadError, 'Error');
    //   this.isLoading = false;
    //   return;
    // }
    // const po = result.data;

    // ASSUMPTION: mocked for now, the same way PurchaseOrdersComponent's
    // stock-alert panel is currently mocked, so this page can be visually
    // verified before the backend endpoint exists. Delete this block and
    // uncomment the real call above once getPurchaseOrderByIdAsync exists.
    const po = this.getMockPurchaseOrder(id);

    if (po.orderStatus !== 'Draft') {
      this.toastr.error('Only Draft purchase orders can be edited.', 'Not Editable');
      this.router.navigate(['admin', 'purchase-orders']);
      return;
    }

    this.purchaseOrderCode = po.purchaseOrderCode;
    this.supplierName = po.supplierName;
    this.expectedDeliveryDate = new Date(po.expectedDeliveryDate).toISOString().split('T')[0];

    // ASSUMPTION: PurchaseOrderDTO doesn't carry a supplierId field directly
    // (only supplierName). Reading it off the first line item's
    // originalSupplierId instead — every item on a PO shares one supplier by
    // design. If this draft somehow has zero items, there's no way to know
    // whose clothes to offer, so we stop and flag it rather than guess.
    // Worth considering: adding SupplierId directly to PurchaseOrderDTO so
    // this workaround isn't needed.
    const firstItem = po.purchaseOrderItems[0];
    if (!firstItem) {
      this.loadError = "This draft has no items yet, so its supplier can't be determined.";
      this.toastr.error(this.loadError, 'Error');
      this.isLoading = false;
      return;
    }
    this.supplierId = firstItem.originalSupplierId;

    this.lineItems = po.purchaseOrderItems.map((item) => ({
      clothe: item.clothe,
      orderedQuantity: item.orderedQuantity,
      // Existing items keep their already-locked unitCost as-is — it is NOT
      // re-derived from Clothe.UnitCost, same reasoning as Create: a locked
      // snapshot must not silently drift if Clothe.UnitCost has changed since.
      unitCost: item.unitCost,
      isExisting: true,
    }));

    await this.loadClothesForSupplier(this.supplierId);
    this.isLoading = false;
  }

  // ASSUMPTION: placeholder only — delete once loadPurchaseOrder calls the
  // real service method above.
  private getMockPurchaseOrder(id: number): PurchaseOrderDTO {
    return {
      id,
      purchaseOrderCode: `PO-${id.toString().padStart(4, '0')}`,
      orderDate: new Date(),
      expectedDeliveryDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
      supplierName: 'Mock Supplier',
      orderStatus: 'Draft',
      employeeName: 'Mock Employee',
      totalAmount: 0,
      purchaseOrderItems: [],
    };
  }

  // ── Clothes for the fixed supplier ──────────────────────────────────────
  async loadClothesForSupplier(supplierId: number): Promise<void> {
    this.isClothesLoading = true;

    this.supplierService.getAllSupplierClothesByIdAsync(supplierId)
      .then((res) => {
        if (!res.isSuccess) {
          this.availableClothes = [];
          return;
        }
        this.availableClothes = res.data ?? [];
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
          // unitCost is a snapshot of Clothe.UnitCost at line-item creation
          // time — read-only in the UI, same as Create.
          unitCost: clothe.unitCost,
          isExisting: false,
        },
      ];
    } else {
      this.lineItems = this.lineItems.filter((li) => li.clothe.id !== clothe.id);
    }
  }

  removeLineItem(clotheId: number): void {
    this.lineItems = this.lineItems.filter((li) => li.clothe.id !== clotheId);
  }

  // ── Totals ─────────────────────────────────────────────────────────────────
  get runningTotal(): number {
    return this.lineItems.reduce((sum, li) => sum + li.orderedQuantity * li.unitCost, 0);
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

    const payload = {
      purchaseOrderId: this.purchaseOrderId,
      expectedDeliveryDate: this.expectedDeliveryDate,
      targetStatus: mode === 'draft' ? 0 : 1, // 0 = Draft, 1 = Ordered
      items: this.lineItems.map((li) => ({
        clotheId: li.clothe.id,
        orderedQuantity: li.orderedQuantity,
        unitCost: li.unitCost,
      })),
    };

    // TODO: UPDATE PURCHASE ORDER — wire up once the backend command exists
    // const result = await this.purchaseOrderService.updatePurchaseOrderAsync(payload);
    // if (result.isSuccess) {
    //   this.toastr.success(
    //     mode === 'draft'
    //       ? 'Draft updated.'
    //       : 'Purchase order placed successfully.',
    //     mode === 'draft' ? 'Draft Updated' : 'Order Placed'
    //   );
    //   this.router.navigate(['admin', 'purchase-orders']);
    // } else {
    //   this.toastr.error(result.errorMessage ?? 'Something went wrong.', 'Error');
    // }

    this.isSubmitting = false;
  }

  cancel(): void {
    // Nothing was persisted mid-edit, so Cancel is just a navigate-back —
    // there's nothing to revert.
    this.router.navigate(['admin', 'purchase-orders']);
  }
}