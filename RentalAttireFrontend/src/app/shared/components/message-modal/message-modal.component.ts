import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export type MessageModalType = 'confirm' | 'info';
export type MessageModalVariant = 'success' | 'error' | 'warning' | 'neutral';

@Component({
  selector: 'app-message-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './message-modal.component.html',
  styleUrl: './message-modal.component.scss'
})
export class MessageModalComponent {

  // === INPUTS ===
  @Input() isOpen: boolean = false;
  @Input() type: MessageModalType = 'info';
  @Input() variant: MessageModalVariant = 'neutral';
  @Input() title: string = '';
  @Input() message: string = '';
  @Input() confirmLabel: string = 'Confirm';
  @Input() cancelLabel: string = 'Cancel';

  // === OUTPUTS ===
  @Output() confirmed = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  // === METHODS ===
  // No isOpen mutation in here — the parent owns that state and is
  // responsible for flipping it in its (confirmed)/(closed) handlers.

  onConfirm(): void {
    this.confirmed.emit();
  }

  onCancel(): void {
    this.closed.emit();
  }

  onOverlayClick(): void {
    this.closed.emit();
  }
}