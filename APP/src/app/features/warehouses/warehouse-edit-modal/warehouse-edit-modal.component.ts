import { Component, input, output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { WarehousesService } from '../warehouses.service';
import { Warehouse, UpdateWarehouseDto } from '@core/models/warehouse.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-warehouse-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './warehouse-edit-modal.component.html',
  styleUrls: ['./warehouse-edit-modal.component.scss']
})
export class WarehouseEditModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly warehousesService = inject(WarehousesService);
  protected readonly i18n = inject(I18nService);
  
  warehouse = input.required<Warehouse>();
  closeModal = output<void>();
  warehouseUpdated = output<void>();
  
  isOpen = true;
  isSaving = false;
  
  form: FormGroup = this.fb.group({
    name: ['', [Validators.required]],
    code: ['', [Validators.required]],
    address: ['', [Validators.required]],
    city: ['', [Validators.required]],
    state: ['', [Validators.required]],
    zipCode: ['', [Validators.required]],
    country: ['Brasil'],
    isActive: [true]
  });
  
  ngOnInit(): void {
    const w = this.warehouse();
    this.form.patchValue({
      name: w.name,
      code: w.code,
      address: w.address,
      city: w.city,
      state: w.state,
      zipCode: w.zipCode,
      country: w.country || 'Brasil',
      isActive: w.isActive
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
      const data: UpdateWarehouseDto = this.form.value;
      await this.warehousesService.update(this.warehouse().id, data);
      this.warehouseUpdated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao atualizar armazém:', error);
      alert(this.i18n.t('common.errors.updateWarehouse'));
    } finally {
      this.isSaving = false;
    }
  }
}
