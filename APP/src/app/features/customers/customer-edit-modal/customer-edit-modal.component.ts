import { Component, input, output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { CustomersService } from '../customers.service';
import { Customer, UpdateCustomerDto } from '@core/models/customer.model';

@Component({
  selector: 'app-customer-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './customer-edit-modal.component.html',
  styleUrls: ['./customer-edit-modal.component.scss']
})
export class CustomerEditModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly customersService = inject(CustomersService);
  
  customer = input.required<Customer>();
  closeModal = output<void>();
  customerUpdated = output<void>();
  
  isOpen = true;
  isSaving = false;
  
  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required]],
    document: ['', [Validators.required]],
    address: ['', [Validators.required]],
    city: ['', [Validators.required]],
    state: ['', [Validators.required]],
    zipCode: ['', [Validators.required]],
    country: ['Brasil'],
    isActive: [true]
  });
  
  ngOnInit(): void {
    const c = this.customer();
    this.form.patchValue({
      name: c.name,
      email: c.email,
      phone: c.phone,
      document: c.document,
      address: c.address,
      city: c.city,
      state: c.state,
      zipCode: c.zipCode,
      country: c.country || 'Brasil',
      isActive: c.isActive
    });
  }
  
  onClose(): void {
    this.closeModal.emit();
  }
  
  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      Object.keys(this.form.controls).forEach(key => {
        this.form.get(key)?.markAsTouched();
      });
      return;
    }
    
    this.isSaving = true;
    try {
      const data: UpdateCustomerDto = this.form.value;
      await this.customersService.update(this.customer().id, data);
      this.customerUpdated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao atualizar cliente:', error);
      alert('Erro ao atualizar cliente');
    } finally {
      this.isSaving = false;
    }
  }
}
