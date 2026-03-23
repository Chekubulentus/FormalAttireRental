import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ClotheDTO } from '../../../../data/models/DTOs/Clothes/clothes';

@Component({
  selector: 'app-view-clothe',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './view-clothe.component.html',
  styleUrl: './view-clothe.component.scss',
})
export class ViewClotheComponent {

  @Input() clothe: ClotheDTO = new ClotheDTO();
  @Output() closed = new EventEmitter<void>();

  // ── Image zoom state ───────────────────────────────────────
  isZoomed    = false;
  imgError    = false;

  openZoom(): void  { if (!this.imgError) this.isZoomed = true; }
  closeZoom(): void { this.isZoomed = false; }

  onImgError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
    this.imgError = true;
  }

  close(): void {
    this.closed.emit();
  }

  // ── Helpers ────────────────────────────────────────────────

  getStockStatus(): 'available' | 'low' | 'out' {
    if (this.clothe.availableQuantity === 0)                               return 'out';
    if (this.clothe.availableQuantity <= this.clothe.stockQuantity * 0.2) return 'low';
    return 'available';
  }

  getStockLabel(): string {
    const map = { available: 'Available', low: 'Low Stock', out: 'Out of Stock' };
    return map[this.getStockStatus()];
  }

  getConditionClass(): string {
    const map: Record<string, string> = {
      new: 'cond--new', good: 'cond--good', fair: 'cond--fair', poor: 'cond--poor',
    };
    return map[this.clothe.condition?.toLowerCase()] ?? '';
  }

  get stockPercent(): number {
    if (!this.clothe.stockQuantity) return 0;
    return Math.round((this.clothe.availableQuantity / this.clothe.stockQuantity) * 100);
  }
}