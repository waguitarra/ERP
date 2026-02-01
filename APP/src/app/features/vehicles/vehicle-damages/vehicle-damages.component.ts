import { Component, inject, Input, OnInit, signal, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../vehicle-management.service';
import { VehicleDamage, DamageType, DamageSeverity, DamageStatus, CreateDamageRequest, RepairDamageRequest, DamageInsuranceRequest, DamagePhotosRequest } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-vehicle-damages',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-damages.component.html',
  styleUrls: ['./vehicle-damages.component.scss']
})
export class VehicleDamagesComponent implements OnInit {
  @Input({ required: true }) vehicleId!: string;
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  protected readonly i18n = inject(I18nService);
  private readonly service = inject(VehicleManagementService);

  damages = signal<VehicleDamage[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = signal(false);
  showPhotoModal = signal(false);
  editingDamage = signal<VehicleDamage | null>(null);
  selectedDamage = signal<VehicleDamage | null>(null);
  saving = signal(false);
  uploadingPhotos = signal(false);

  formData: CreateDamageRequest = this.getEmptyForm();
  selectedPhotos: File[] = [];
  previewUrls: string[] = [];

  damageTypes = Object.entries(DamageType)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  severityLevels = Object.entries(DamageSeverity)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  damageStatuses = Object.entries(DamageStatus)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  ngOnInit(): void {
    this.loadDamages();
  }

  async loadDamages(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const response = await this.service.getDamages(this.vehicleId);
      this.damages.set(response.data || []);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar avarias');
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void {
    this.formData = this.getEmptyForm();
    this.editingDamage.set(null);
    this.selectedPhotos = [];
    this.previewUrls = [];
    this.showModal.set(true);
  }

  openEditModal(d: VehicleDamage): void {
    this.formData = {
      type: d.type,
      severity: d.severity,
      title: d.title,
      description: d.description,
      occurrenceDate: d.occurrenceDate.split('T')[0],
      damageLocation: d.damageLocation,
      mileageAtOccurrence: d.mileageAtOccurrence,
      estimatedRepairCost: d.estimatedRepairCost,
      driverName: d.driverName,
      isThirdPartyFault: d.isThirdPartyFault,
      thirdPartyInfo: d.thirdPartyInfo,
      notes: d.notes
    };
    this.editingDamage.set(d);
    this.selectedPhotos = [];
    this.previewUrls = d.photoUrls ? [...d.photoUrls] : [];
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingDamage.set(null);
    this.selectedPhotos = [];
    this.previewUrls = [];
  }

  openPhotoModal(damage: VehicleDamage): void {
    this.selectedDamage.set(damage);
    this.showPhotoModal.set(true);
  }

  closePhotoModal(): void {
    this.showPhotoModal.set(false);
    this.selectedDamage.set(null);
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      const newFiles = Array.from(input.files);
      this.selectedPhotos.push(...newFiles);
      
      newFiles.forEach(file => {
        const reader = new FileReader();
        reader.onload = () => {
          this.previewUrls = [...this.previewUrls, reader.result as string];
        };
        reader.readAsDataURL(file);
      });
    }
  }

  removePhoto(index: number): void {
    if (index < this.selectedPhotos.length) {
      this.selectedPhotos.splice(index, 1);
    }
    this.previewUrls.splice(index, 1);
    this.previewUrls = [...this.previewUrls];
  }

  triggerFileInput(): void {
    this.fileInput.nativeElement.click();
  }

  async saveDamage(): Promise<void> {
    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.saving.set(true);

    try {
      const editing = this.editingDamage();
      let damageId: string;
      
      if (editing) {
        await this.service.updateDamage(this.vehicleId, editing.id, this.formData);
        damageId = editing.id;
      } else {
        const response = await this.service.createDamage(this.vehicleId, this.formData);
        damageId = response.data.id;
      }

      // Upload new photos if any
      const newPhotos = this.previewUrls.filter(url => url.startsWith('data:'));
      if (newPhotos.length > 0) {
        await this.service.updateDamagePhotos(this.vehicleId, damageId, { photoUrls: newPhotos });
      }

      this.closeModal();
      await this.loadDamages();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao salvar avaria');
    } finally {
      this.saving.set(false);
    }
  }

  async deleteDamage(d: VehicleDamage): Promise<void> {
    if (!confirm(this.i18n.t('common.confirmDelete'))) return;

    try {
      await this.service.deleteDamage(this.vehicleId, d.id);
      await this.loadDamages();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao excluir avaria');
    }
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number | undefined): string {
    if (value === undefined) return '-';
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'EUR' }).format(value);
  }

  getDamageTypeLabel(type: DamageType): string {
    return this.i18n.t(`vehicleDetail.damages.types.${DamageType[type]?.toLowerCase()}`) || DamageType[type];
  }

  getSeverityLabel(severity: DamageSeverity): string {
    return this.i18n.t(`vehicleDetail.damages.severity.${DamageSeverity[severity]?.toLowerCase()}`) || DamageSeverity[severity];
  }

  getStatusLabel(status: DamageStatus): string {
    return this.i18n.t(`vehicleDetail.damages.status.${DamageStatus[status]?.toLowerCase()}`) || DamageStatus[status];
  }

  getSeverityBadge(severity: DamageSeverity): string {
    const badges: Record<number, string> = {
      [DamageSeverity.Minor]: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
      [DamageSeverity.Moderate]: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
      [DamageSeverity.Major]: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-300',
      [DamageSeverity.Critical]: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    };
    return badges[severity] || 'bg-gray-100 text-gray-800';
  }

  getStatusBadge(status: DamageStatus): string {
    const badges: Record<number, string> = {
      [DamageStatus.Reported]: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
      [DamageStatus.UnderAssessment]: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
      [DamageStatus.UnderRepair]: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300',
      [DamageStatus.Repaired]: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
      [DamageStatus.WriteOff]: 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-300'
    };
    return badges[status] || 'bg-gray-100 text-gray-800';
  }

  private getEmptyForm(): CreateDamageRequest {
    return {
      type: DamageType.Collision,
      severity: DamageSeverity.Minor,
      title: '',
      description: '',
      occurrenceDate: new Date().toISOString().split('T')[0],
      mileageAtOccurrence: 0
    };
  }
}
