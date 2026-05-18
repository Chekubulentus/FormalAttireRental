import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { DisposablesService } from '../disposables-service/disposables.service';
import { ArchivedEntity } from '../../../../data/models/DTOs/Disposables/archive-entity';

@Component({
  selector: 'app-view-record',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './view-record.component.html',
  styleUrl: './view-record.component.scss',
})
export class ViewRecordComponent implements OnInit {

  @Input() record: ArchivedEntity = new ArchivedEntity();
  @Output() closed = new EventEmitter<void>();

  // ── State ──────────────────────────────────────────────────
  isLoading = false;
  imgError  = false;

  // ── Fetched data — only one will be populated ──────────────
  // res.data = ViewRecordResponse { entityType, record: object }
  // res.data.record is the actual DTO (EmployeeDTO, CustomerDTO, etc.)
  employee: any = null;
  customer: any = null;
  clothe:   any = null;
  category: any = null;

  constructor(private disposablesService: DisposablesService) {}

  ngOnInit(): void {
    this.fetchRecord();
  }

  fetchRecord(): void {
    this.isLoading = true;

    this.disposablesService
      .viewArchivedRecordAsync(this.record.entityId, this.record.entityType)
      .then((res) => {
        if (!res.isSuccess) {
          console.error('ViewRecord error:', res.errorMessage);
          return;
        }

        // res.data is ViewRecordResponse — the actual DTO is in res.data.record
        const data = res.data as any;
        const dto  = data?.record;

        switch (this.record.entityType) {
          case 'Employee': this.employee = dto; break;
          case 'Customer': this.customer = dto; break;
          case 'Clothe':   this.clothe   = dto; break;
          case 'Category': this.category = dto; break;
        }
      })
      .catch((err) => console.error('ViewRecord fetch error:', err))
      .finally(() => (this.isLoading = false));
  }

  close(): void {
    this.closed.emit();
  }

  onImgError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
    this.imgError = true;
  }

  // ── Helpers ────────────────────────────────────────────────
  getInitials(fullName: string): string {
    const parts = fullName?.trim().split(' ') ?? [];
    if (!parts.length) return '?';
    if (parts.length === 1) return parts[0][0]?.toUpperCase() ?? '?';
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }

  private avatarColors = [
    '#a07840', '#7a9e7e', '#8b7aad', '#4a90a4',
    '#c0697a', '#6b8e6b', '#a0522d', '#5a7a9e',
  ];

  getAvatarColor(id: number): string {
    return this.avatarColors[id % this.avatarColors.length];
  }

  getStockStatus(clothe: any): 'available' | 'low' | 'out' {
    if (!clothe?.availableQuantity) return 'out';
    if (clothe.availableQuantity <= clothe.stockQuantity * 0.2) return 'low';
    return 'available';
  }

  getStockLabel(clothe: any): string {
    const map = { available: 'Available', low: 'Low Stock', out: 'Out of Stock' };
    return map[this.getStockStatus(clothe)];
  }

  getConditionClass(condition: string): string {
    const map: Record<string, string> = {
      new:  'cond--new',
      good: 'cond--good',
      fair: 'cond--fair',
      poor: 'cond--poor',
    };
    return map[condition?.toLowerCase()] ?? '';
  }
}