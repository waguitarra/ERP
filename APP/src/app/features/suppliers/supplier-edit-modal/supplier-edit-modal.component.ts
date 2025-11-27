import { Component, input, output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { SuppliersService } from '../suppliers.service';
import { Supplier, UpdateSupplierDto } from '@core/models/supplier.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-supplier-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './supplier-edit-modal.component.html',
  styleUrls: ['./supplier-edit-modal.component.scss']
})
export class SupplierEditModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly suppliersService = inject(SuppliersService);
  protected readonly i18n = inject(I18nService);
  
  supplier = input.required<Supplier>();
  closeModal = output<void>();
  supplierUpdated = output<void>();
  
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
    const s = this.supplier();
    this.form.patchValue({
      name: s.name,
      email: s.email,
      phone: s.phone,
      document: s.document,
      address: s.address,
      city: s.city,
      state: s.state,
      zipCode: s.zipCode,
      country: s.country || 'Brasil',
      isActive: s.isActive
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
      const data: UpdateSupplierDto = this.form.value;
      await this.suppliersService.update(this.supplier().id, data);
      this.supplierUpdated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao atualizar fornecedor:', error);
      alert('Erro ao atualizar fornecedor');
    } finally {
      this.isSaving = false;
    }
  }
}
