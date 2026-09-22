import { Component, EventEmitter, HostListener, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { PurchaseOrderService } from '../purchase-order-service/purchase-order.service';
import { PurchaseOrderDTO } from '../dtos/purchase-order-dto';

// ── Local working type for receiving history ───────────────────────────────
// ASSUMPTION: ReceivingBatch isn't on PurchaseOrderDTO yet, so this is a
// standalone shape for the mocked data below. Replace once the backend
// decides where this actually comes from (nested on PurchaseOrderDTO, or its
// own endpoint keyed by purchaseOrderId).
interface ReceivingHistoryEntry {
  receivingBatchCode: string;
  receivedDate: Date;
  employeeName: string;
  items: { clotheName: string; quantityReceived: number }[];
}

@Component({
  selector: 'app-view-purchase-order',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './view-purchase-order.component.html',
  styleUrl: './view-purchase-order.component.scss',
})
export class ViewPurchaseOrderComponent implements OnChanges {
  @Input() purchaseOrderId: number | null = null;
  @Input() isOpen = false;
  @Output() closed = new EventEmitter<void>();

  purchaseOrder: PurchaseOrderDTO | null = null;
  receivingHistory: ReceivingHistoryEntry[] = [];
  isLoading = false;
  loadError = '';

  constructor(
    private purchaseOrderService: PurchaseOrderService,
    private toastr: ToastrService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    // Fetch when the modal opens with a valid id, or the id changes while
    // already open (e.g. a parent list re-targets the modal without closing
    // it first). Guarding on the actual value change — rather than just
    // "isOpen is true" — avoids refetching on every unrelated change
    // detection pass while the modal happens to still be open.
    const idChanged =
      !!changes['purchaseOrderId'] &&
      changes['purchaseOrderId'].currentValue !== changes['purchaseOrderId'].previousValue;
    const justOpened = !!changes['isOpen'] && changes['isOpen'].currentValue === true;

    if (this.isOpen && this.purchaseOrderId && (idChanged || justOpened)) {
      this.loadPurchaseOrder(this.purchaseOrderId);
    }
  }

  // ASSUMPTION: closing on Escape — not specified, but standard modal
  // behavior and cheap to add; remove if it doesn't match an existing
  // modal's convention elsewhere in the app.
  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.isOpen) this.close();
  }

  async loadPurchaseOrder(id: number): Promise<void> {
    this.isLoading = true;
    this.loadError = '';
    this.purchaseOrder = null;
    this.receivingHistory = [];

    // TODO: GET PURCHASE ORDER BY ID — wire up once the backend query exists
    // const result = await this.purchaseOrderService.getPurchaseOrderByIdAsync(id);
    // if (!result.isSuccess || !result.data) {
    //   this.loadError = result.errorMessage ?? 'Purchase order not found.';
    //   this.toastr.error(this.loadError, 'Error');
    //   this.isLoading = false;
    //   return;
    // }
    // this.purchaseOrder = result.data;

    // TODO: GET RECEIVING HISTORY — separate call or nested field, TBD
    // this.receivingHistory = result.data.receivingHistory ?? [];

    // ASSUMPTION: mocked for now, same reasoning as EditPurchaseOrderComponent
    // — getPurchaseOrderByIdAsync doesn't exist on PurchaseOrderService yet.
    this.purchaseOrder = this.getMockPurchaseOrder(id);
    this.receivingHistory = this.getMockReceivingHistory();

    this.isLoading = false;
  }

  // ASSUMPTION: placeholder only — delete once loadPurchaseOrder calls the
  // real service method above.
  private getMockPurchaseOrder(id: number): PurchaseOrderDTO {
    return {
      id,
      purchaseOrderCode: `PO-${id.toString().padStart(4, '0')}`,
      orderDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
      expectedDeliveryDate: new Date(Date.now() + 4 * 24 * 60 * 60 * 1000),
      supplierName: 'Mock Supplier',
      orderStatus: 'PartiallyReceived',
      employeeName: 'Mock Employee',
      totalAmount: 7500,
      purchaseOrderItems: [
        {
          id: 1,
          purchaseOrderId: id,
          clothe: {
            id: 1,
            clotheCode: 'CLO-001',
            clotheName: 'Classic Black Barong',
            categoryName: 'Barong',
            color: 'Black',
            brand: '',
            material: 'Piña',
            size: 'L',
            clotheGender: 'Male',
            stockQuantity: 10,
            availableQuantity: 6,
            rentalPrice: 800,
            depositAmount: 500,
            rentalDurationDays: 3,
            condition: 'Good',
            reservedQuantity: 4,
            isAvailable: true,
            rentalCount: 12,
            unitCost: 750,
            profileImagePath: '',
            supplier: null,
          },
          orderedQuantity: 10,
          receivedQuantity: 6,
          unitCost: 750,
          originalSupplierId: 1,
          totalAmount: 7500,
        },
      ],
    };
  }

  private getMockReceivingHistory(): ReceivingHistoryEntry[] {
    return [
      {
        receivingBatchCode: 'RB-0001',
        receivedDate: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
        employeeName: 'Mock Employee',
        items: [{ clotheName: 'Classic Black Barong', quantityReceived: 6 }],
      },
    ];
  }

  // ── Status badge helpers ────────────────────────────────────────────────
  // ASSUMPTION: color mapping guessed from the app's existing $red/$green/
  // $amber/$blue status-color tokens — not confirmed against
  // PurchaseOrdersComponent's actual inline badge, which I haven't seen.
  // Reconcile these when that component's badge styling gets extracted into
  // the shared PurchaseOrderStatusBadge you flagged.
  statusClass(status: string): string {
    switch (status) {
      case 'Draft': return 'status-badge--draft';
      case 'Ordered': return 'status-badge--ordered';
      case 'PartiallyReceived': return 'status-badge--partial';
      case 'Received': return 'status-badge--received';
      case 'Cancelled': return 'status-badge--cancelled';
      default: return '';
    }
  }

  statusLabel(status: string): string {
    return status === 'PartiallyReceived' ? 'Partially Received' : status;
  }

  close(): void {
    this.closed.emit();
  }

  onBackdropClick(event: MouseEvent): void {
    if (event.target === event.currentTarget) this.close();
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-PH', {
      style: 'currency',
      currency: 'PHP',
      minimumFractionDigits: 2,
    }).format(amount);
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-PH', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }
}