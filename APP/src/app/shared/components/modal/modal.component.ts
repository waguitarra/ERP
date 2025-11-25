import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss']
})
export class ModalComponent {
  isOpen = input<boolean>(false);
  title = input<string>('');
  size = input<'sm' | 'md' | 'lg' | 'xl'>('md');
  
  close = output<void>();
  
  onClose(): void {
    this.close.emit();
  }
  
  onBackdropClick(): void {
    this.onClose();
  }
  
  onContentClick(event: Event): void {
    event.stopPropagation();
  }
}
