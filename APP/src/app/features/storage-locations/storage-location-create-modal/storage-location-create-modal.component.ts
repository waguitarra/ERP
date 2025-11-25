import { Component, signal, inject, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { StorageLocationsService } from '../storage-locations.service';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-storage-location-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './storage-location-create-modal.component.html',
  styleUrls: ['./storage-location-create-modal.component.scss']
})
export class StorageLocationCreateModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly storageLocationsService = inject(StorageLocationsService);
  protected readonly i18n = inject(I18nService);

  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  
  locationCreated = output<void>();
  
  form: FormGroup = this.fb.group({
    warehouseId: ['', [Validators.required]],
    zoneId: [''],
    code: ['', [Validators.required]],
    aisle: ['', [Validators.required]],
    rack: ['', [Validators.required]],
    shelf: ['', [Validators.required]],
    bin: [''],
    locationType: ['Standard', [Validators.required]],
    capacity: [null]
  });

  locationTypes = ['Standard', 'Picking', 'Receiving', 'Shipping', 'Returns', 'Quarantine'];

  open(): void {
    this.isOpen.set(true);
    this.form.reset({ locationType: 'Standard' });
  }

  close(): void {
    this.isOpen.set(false);
    this.form.reset();
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    try {
      await this.storageLocationsService.create(this.form.value);
      this.locationCreated.emit();
      this.close();
    } catch (error) {
      console.error('Erro ao criar localização:', error);
      alert('Erro ao criar localização');
    } finally {
      this.loading.set(false);
    }
  }

  getError(fieldName: string): string {
    const field = this.form.get(fieldName);
    if (field?.hasError('required')) return 'Campo obrigatório';
    return '';
  }
}
