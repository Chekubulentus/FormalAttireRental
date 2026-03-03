import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-archive-confirmation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './archive-confirmation.component.html',
  styleUrl: './archive-confirmation.component.scss',
})
export class ArchiveConfirmationComponent {
  @Input() recordName = '';
  @Input() recordType = '';
  @Input() isArchiving = false;
  @Output() cancelled = new EventEmitter<void>();
  @Output() confirmed = new EventEmitter<void>();

  cancel(): void {
    this.cancelled.emit();
  }

  confirm(): void {
    this.confirmed.emit();
  }
}