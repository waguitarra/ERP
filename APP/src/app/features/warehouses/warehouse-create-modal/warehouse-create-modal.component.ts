import { Component, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { WarehousesService } from '../warehouses.service';
import { AuthService } from '@core/services/auth.service';
import { CreateWarehouseDto } from '@core/models/warehouse.model';

@Component({
  selector: 'app-warehouse-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './warehouse-create-modal.component.html',
  styleUrls: ['./warehouse-create-modal.component.scss']
})
export class WarehouseCreateModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly warehousesService = inject(WarehousesService);
  private readonly authService = inject(AuthService);
  
  closeModal = output<void>();
  warehouseCreated = output<void>();
  
  isOpen = true;
  isSaving = false;
  
  form: FormGroup = this.fb.group({
    name: ['', [Validators.required]],
    code: ['', [Validators.required]],
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
      
      const data: CreateWarehouseDto = {
        companyId,
        ...this.form.value
      };
      
      await this.warehousesService.create(data);
      this.warehouseCreated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao criar armazém:', error);
      alert('Erro ao criar armazém');
    } finally {
      this.isSaving = false;
    }
  }
}
