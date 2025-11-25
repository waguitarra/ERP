import { Component, signal, inject, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { StorageLocationsService } from '../storage-locations.service';
import { StorageLocation } from '@core/models/storage-location.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-storage-location-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './storage-location-edit-modal.component.html',
  styleUrls: ['./storage-location-edit-modal.component.scss']
})
export class StorageLocationEditModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly storageLocationsService = inject(StorageLocationsService);
  protected readonly i18n = inject(I18nService);

  location = input.required<StorageLocation>();
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  
  locationUpdated = output<void>();
  
  form: FormGroup = this.fb.group({
    code: ['', [Validators.required]],
    aisle: [''],
    rack: [''],
    shelf: [''],
    bin: [''],
    locationType: ['', [Validators.required]],
    capacity: [null],
    isActive: [true]
  });

  locationTypes = ['Standard', 'Picking', 'Receiving', 'Shipping', 'Returns', 'Quarantine'];

  open(): void {
    const loc = this.location();
    this.form.patchValue({
      code: loc.code,
      aisle: loc.aisle,
      rack: loc.rack,
      shelf: loc.shelf,
      bin: loc.bin,
      locationType: loc.locationType,
      capacity: loc.capacity,
      isActive: loc.isActive
    });
    this.isOpen.set(true);
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
      await this.storageLocationsService.update(this.location().id, this.form.value);
      this.locationUpdated.emit();
      this.close();
    } catch (error) {
      console.error('Erro ao atualizar localização:', error);
      alert('Erro ao atualizar localização');
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
