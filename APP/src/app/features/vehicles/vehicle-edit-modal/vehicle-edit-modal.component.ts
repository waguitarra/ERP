import { Component, input, output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { VehiclesService } from '../vehicles.service';
import { Vehicle, UpdateVehicleDto } from '@core/models/vehicle.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-vehicle-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './vehicle-edit-modal.component.html',
  styleUrls: ['./vehicle-edit-modal.component.scss']
})
export class VehicleEditModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly vehiclesService = inject(VehiclesService);
  protected readonly i18n = inject(I18nService);
  
  vehicle = input.required<Vehicle>();
  closeModal = output<void>();
  vehicleUpdated = output<void>();
  
  isOpen = true;
  isSaving = false;
  
  form: FormGroup = this.fb.group({
    plateNumber: ['', [Validators.required]],
    vehicleType: ['', [Validators.required]],
    model: ['', [Validators.required]],
    capacity: [0, [Validators.required, Validators.min(0)]],
    status: ['Available'],
    isActive: [true]
  });
  
  ngOnInit(): void {
    const v = this.vehicle();
    this.form.patchValue({
      plateNumber: v.plateNumber,
      vehicleType: v.vehicleType,
      model: v.model,
      capacity: v.capacity,
      status: v.status,
      isActive: v.isActive
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
      const data: UpdateVehicleDto = this.form.value;
      await this.vehiclesService.update(this.vehicle().id, data);
      this.vehicleUpdated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao atualizar veículo:', error);
      alert('Erro ao atualizar veículo');
    } finally {
      this.isSaving = false;
    }
  }
}
