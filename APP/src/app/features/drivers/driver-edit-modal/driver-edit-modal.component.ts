import { Component, input, output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { DriversService } from '../drivers.service';
import { Driver, UpdateDriverDto } from '@core/models/driver.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-driver-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './driver-edit-modal.component.html',
  styleUrls: ['./driver-edit-modal.component.scss']
})
export class DriverEditModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly driversService = inject(DriversService);
  protected readonly i18n = inject(I18nService);
  
  driver = input.required<Driver>();
  closeModal = output<void>();
  driverUpdated = output<void>();
  
  isOpen = true;
  isSaving = false;
  
  form: FormGroup = this.fb.group({
    name: ['', [Validators.required]],
    licenseNumber: ['', [Validators.required]],
    phone: ['', [Validators.required]],
    email: ['', [Validators.email]],
    isActive: [true]
  });
  
  ngOnInit(): void {
    const d = this.driver();
    this.form.patchValue({
      name: d.name,
      licenseNumber: d.licenseNumber,
      phone: d.phone,
      email: d.email,
      isActive: d.isActive
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
      const data: UpdateDriverDto = this.form.value;
      await this.driversService.update(this.driver().id, data);
      this.driverUpdated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao atualizar motorista:', error);
      alert(this.i18n.t('common.errors.updateDriver'));
    } finally {
      this.isSaving = false;
    }
  }
}
