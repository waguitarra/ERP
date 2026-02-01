import { Component, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { VehiclesService } from '../vehicles.service';
import { AuthService } from '@core/services/auth.service';
import { CreateVehicleDto } from '@core/models/vehicle.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-vehicle-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './vehicle-create-modal.component.html',
  styleUrls: ['./vehicle-create-modal.component.scss']
})
export class VehicleCreateModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly vehiclesService = inject(VehiclesService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);
  
  closeModal = output<void>();
  vehicleCreated = output<void>();
  
  isOpen = true;
  isSaving = false;
  
  form: FormGroup = this.fb.group({
    licensePlate: ['', [Validators.required, Validators.pattern(/^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$/)]],
    brand: [''],
    model: ['', [Validators.required]],
    vehicleType: [''],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
    capacity: [null],
    color: [''],
    fuelType: [''],
    notes: [''],
    trackingEnabled: [false]
  });

  vehicleTypes = [
    { value: 'Truck', label: 'Caminhão' },
    { value: 'Van', label: 'Van' },
    { value: 'Car', label: 'Carro' },
    { value: 'Motorcycle', label: 'Motocicleta' },
    { value: 'Pickup', label: 'Pickup' },
    { value: 'Other', label: 'Outro' }
  ];

  fuelTypes = [
    { value: 'Gasoline', label: 'Gasolina' },
    { value: 'Diesel', label: 'Diesel' },
    { value: 'Ethanol', label: 'Etanol' },
    { value: 'Flex', label: 'Flex' },
    { value: 'Electric', label: 'Elétrico' },
    { value: 'Hybrid', label: 'Híbrido' }
  ];
  
  onClose(): void {
    this.closeModal.emit();
  }

  formatLicensePlate(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
    if (value.length > 7) value = value.substring(0, 7);
    this.form.get('licensePlate')?.setValue(value);
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
      
      const formValue = this.form.value;
      const data: CreateVehicleDto = {
        companyId,
        licensePlate: formValue.licensePlate,
        model: formValue.model,
        brand: formValue.brand || undefined,
        vehicleType: formValue.vehicleType || undefined,
        year: formValue.year,
        capacity: formValue.capacity || undefined,
        color: formValue.color || undefined,
        fuelType: formValue.fuelType || undefined,
        notes: formValue.notes || undefined,
        trackingEnabled: formValue.trackingEnabled
      };
      
      await this.vehiclesService.create(data);
      this.vehicleCreated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao criar veículo:', error);
      alert(this.i18n.t('common.errors.createVehicle'));
    } finally {
      this.isSaving = false;
    }
  }
}
