import { Component, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { SuppliersService } from '../suppliers.service';
import { AuthService } from '@core/services/auth.service';
import { CreateSupplierDto } from '@core/models/supplier.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-supplier-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './supplier-create-modal.component.html',
  styleUrls: ['./supplier-create-modal.component.scss']
})
export class SupplierCreateModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly suppliersService = inject(SuppliersService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);
  
  closeModal = output<void>();
  supplierCreated = output<void>();
  
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
    country: ['Brasil']
  });
  
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
      const user = this.authService.currentUser();
      const companyId = user?.companyId;
      
      if (!companyId) {
        alert('Erro: CompanyId não encontrado');
        return;
      }
      
      const data: CreateSupplierDto = {
        companyId,
        ...this.form.value
      };
      
      await this.suppliersService.create(data);
      this.supplierCreated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao criar fornecedor:', error);
      alert('Erro ao criar fornecedor');
    } finally {
      this.isSaving = false;
    }
  }
}
