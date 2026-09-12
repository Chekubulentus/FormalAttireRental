import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SupplierDTO } from '../../../../data/models/DTOs/Supplier/supplier';
import { SupplierService } from '../suppliers-service/supplier.service';
import { AppToastrService } from '../../../../core/services/toastr-service/app-toastr.service';
import { ViewSupplierClothesModalComponent } from '../view-supplier-clothes-modal/view-supplier-clothes-modal.component';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';

@Component({
  selector: 'app-view-supplier-modal',
  standalone: true,
  imports: [CommonModule, ViewSupplierClothesModalComponent],
  templateUrl: './view-supplier-modal.component.html',
  styleUrl: './view-supplier-modal.component.scss'
})
export class ViewSupplierModalComponent implements OnInit {

  @Input() supplierId!: number;
  @Output() closeModal = new EventEmitter<void>();

  constructor(
    private supplierService: SupplierService,
    private toastrService: AppToastrService
  ) {}

  // ── Remote State ───────────────────────────────────────────────────────────
  supplier: SupplierDTO | null = null;
  isLoading = false;

  // ── Clothes Modal ──────────────────────────────────────────────────────────
  isClothesModalOpen = false;

  get clothes(): ClotheDTO[] {
    return this.supplier?.clothesAvailable ?? [];
  }

  // ── Lifecycle ──────────────────────────────────────────────────────────────
  ngOnInit(): void {
    this.loadSupplier();
  }

  private loadSupplier(): void {
    this.isLoading = true;

    this.supplierService.getSupplierByIdAsync(this.supplierId)
      .then(res => {
        if (!res.isSuccess || !res.data) {
          this.toastrService.error(res.errorMessage ?? 'Supplier could not be loaded.');
          this.close();
          return;
        }
        this.supplier = res.data;
      })
      .catch(err => {
        this.toastrService.error(err.error);
        this.close();
      })
      .finally(() => {
        this.isLoading = false;
      });
  }

  // ── Modal ──────────────────────────────────────────────────────────────────
  openClothesModal(): void  { this.isClothesModalOpen = true; }
  closeClothesModal(): void { this.isClothesModalOpen = false; }

  close(): void { this.closeModal.emit(); }

  onOverlayClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close();
    }
  }
}