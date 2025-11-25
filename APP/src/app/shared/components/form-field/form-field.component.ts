import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-form-field',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './form-field.component.html',
  styleUrls: ['./form-field.component.scss']
})
export class FormFieldComponent {
  label = input<string>('');
  control = input.required<AbstractControl>();
  type = input<string>('text');
  placeholder = input<string>('');
  required = input<boolean>(false);
  
  get hasError(): boolean {
    const ctrl = this.control();
    return ctrl.invalid && (ctrl.dirty || ctrl.touched);
  }
  
  get errorMessage(): string {
    const ctrl = this.control();
    if (!ctrl.errors) return '';
    
    if (ctrl.errors['required']) return 'Campo obrigatório';
    if (ctrl.errors['minlength']) return `Mínimo de ${ctrl.errors['minlength'].requiredLength} caracteres`;
    if (ctrl.errors['maxlength']) return `Máximo de ${ctrl.errors['maxlength'].requiredLength} caracteres`;
    if (ctrl.errors['min']) return `Valor mínimo: ${ctrl.errors['min'].min}`;
    if (ctrl.errors['max']) return `Valor máximo: ${ctrl.errors['max'].max}`;
    if (ctrl.errors['email']) return 'Email inválido';
    if (ctrl.errors['pattern']) return 'Formato inválido';
    
    return 'Campo inválido';
  }
  
  onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.control().setValue(value);
  }
  
  onBlur(): void {
    this.control().markAsTouched();
  }
}
