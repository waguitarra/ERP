import { Component, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { DriversService } from '../drivers.service';
import { AuthService } from '@core/services/auth.service';
import { CreateDriverDto } from '@core/models/driver.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-driver-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './driver-create-modal.component.html',
  styleUrls: ['./driver-create-modal.component.scss']
})
export class DriverCreateModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly driversService = inject(DriversService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);
  
  closeModal = output<void>();
  driverCreated = output<void>();
  
  isOpen = true;
  isSaving = false;
  
  form: FormGroup = this.fb.group({
    name: ['', [Validators.required]],
    licenseNumber: ['', [Validators.required]],
    phone: ['', [Validators.required]],
    email: ['', [Validators.email]]
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
    
    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.isSaving = true;
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId;
      
      if (!companyId) {
        alert(this.i18n.t('common.errors.companyIdNotFound'));
        return;
      }
      
      const data: CreateDriverDto = {
        companyId,
        ...this.form.value
      };
      
      await this.driversService.create(data);
      this.driverCreated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao criar motorista:', error);
      alert(this.i18n.t('common.errors.createDriver'));
    } finally {
      this.isSaving = false;
    }
  }
}
