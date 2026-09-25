import { Component, EventEmitter, HostListener, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { PurchaseOrderService } from '../purchase-order-service/purchase-order.service';
import { PurchaseOrderDTO } from '../dtos/purchase-order-dto';

// ── Receiving history shape ────────────────────────────────────────────────────
// TODO: Replace with a real DTO once GET /PurchaseOrder/{id}/receiving-history
// (or equivalent) is built. Field names here are assumptions — reconcile against
// the actual backend response shape when wiring this up.
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
    const idChanged =
      !!changes['purchaseOrderId'] &&
      changes['purchaseOrderId'].currentValue !== changes['purchaseOrderId'].previousValue;
    const justOpened = !!changes['isOpen'] && changes['isOpen'].currentValue === true;

    if (this.isOpen && this.purchaseOrderId && (idChanged || justOpened)) {
      this.loadPurchaseOrder(this.purchaseOrderId);
    }
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.isOpen) this.close();
  }

  async loadPurchaseOrder(id: number): Promise<void> {
    this.isLoading = true;
    this.loadError = '';
    this.purchaseOrder = null;
    this.receivingHistory = [];

    const result = await this.purchaseOrderService.getPurchaseOrderByIdAsync(id);

    if (!result.isSuccess || !result.data) {
      this.loadError = result.errorMessage ?? 'Purchase order not found.';
      this.toastr.error(this.loadError, 'Error');
      this.isLoading = false;
      return;
    }

    this.purchaseOrder = result.data;

    // TODO: Wire receiving history once GET /PurchaseOrder/{id}/receiving-history
    // (or equivalent) is built. Add a getPurchaseOrderReceivingHistoryAsync method
    // to PurchaseOrderService and call it here in parallel or sequence with the
    // main fetch above.
    // Example:
    // const historyResult = await this.purchaseOrderService.getReceivingHistoryAsync(id);
    // if (historyResult.isSuccess && historyResult.data) {
    //   this.receivingHistory = historyResult.data;
    // }
    this.receivingHistory = [];

    this.isLoading = false;
  }

  statusClass(status: string): string {
    switch (status) {
      case 'Draft':             return 'status-badge--draft';
      case 'Ordered':           return 'status-badge--ordered';
      case 'PartiallyReceived': return 'status-badge--partial';
      case 'Received':          return 'status-badge--received';
      case 'Cancelled':         return 'status-badge--cancelled';
      default:                  return '';
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