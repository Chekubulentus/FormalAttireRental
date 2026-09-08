import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

// TODO: Replace with real DTOs from src/app/data/models/DTOs/
interface ClotheDTO {
  id: number;
  clotheCode: string;
  clotheName: string;
  categoryName: string;
  color: string;
  brand: string;
  material: string;
  size: string;
  clotheGender: string;
  stockQuantity: number;
  availableQuantity: number;
  rentalPrice: number;
  depositAmount: number;
  rentalDurationDays: number;
  condition: string;
  profileImagePath?: string;
}

interface SupplierDTO {
  supplierCode: string;
  supplierName: string;
  phoneNumber: string;
  email: string;
  address: string;
  clothesAvailable: ClotheDTO[];
  createdByEmployee: string;
}

@Component({
  selector: 'app-view-supplier-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './view-supplier-modal.component.html',
  styleUrl: './view-supplier-modal.component.scss'
})
export class ViewSupplierModalComponent {

  @Input() supplier!: SupplierDTO;
  @Output() closeModal = new EventEmitter<void>();

  isClothesModalOpen = false;

  close(): void {
    this.closeModal.emit();
  }

  onOverlayClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close();
    }
  }

  openClothesModal(): void {
    this.isClothesModalOpen = true;
  }

  closeClothesModal(): void {
    this.isClothesModalOpen = false;
  }
}