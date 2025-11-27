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
    plateNumber: ['', [Validators.required]],
    vehicleType: ['', [Validators.required]],
    model: ['', [Validators.required]],
    capacity: [0, [Validators.required, Validators.min(0)]],
    status: ['Available']
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
      
      const data: CreateVehicleDto = {
        companyId,
        ...this.form.value
      };
      
      await this.vehiclesService.create(data);
      this.vehicleCreated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao criar veículo:', error);
      alert('Erro ao criar veículo');
    } finally {
      this.isSaving = false;
    }
  }
}
