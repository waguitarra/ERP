import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../vehicle-management.service';
import { VehicleInspection, InspectionType, InspectionResult, CreateInspectionRequest } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-vehicle-inspections',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-inspections.component.html',
  styleUrls: ['./vehicle-inspections.component.scss']
})
export class VehicleInspectionsComponent implements OnInit {
  @Input({ required: true }) vehicleId!: string;

  protected readonly i18n = inject(I18nService);
  private readonly service = inject(VehicleManagementService);

  inspections = signal<VehicleInspection[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = signal(false);
  editingInspection = signal<VehicleInspection | null>(null);
  saving = signal(false);

  formData: CreateInspectionRequest = this.getEmptyForm();

  inspectionTypes = Object.entries(InspectionType)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  inspectionResults = Object.entries(InspectionResult)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  ngOnInit(): void {
    this.loadInspections();
  }

  async loadInspections(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const response = await this.service.getInspections(this.vehicleId);
      this.inspections.set(response.data || []);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar inspeções');
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void {
    this.formData = this.getEmptyForm();
    this.editingInspection.set(null);
    this.showModal.set(true);
  }

  openEditModal(i: VehicleInspection): void {
    this.formData = {
      type: i.type,
      inspectionDate: i.inspectionDate.split('T')[0],
      expiryDate: i.expiryDate?.split('T')[0] || '',
      result: i.result,
      inspectionCenter: i.inspectionCenter,
      inspectorName: i.inspectorName,
      certificateNumber: i.certificateNumber,
      mileageAtInspection: i.mileageAtInspection,
      cost: i.cost,
      observations: i.observations,
      defectsFound: i.defectsFound
    };
    this.editingInspection.set(i);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingInspection.set(null);
  }

  async saveInspection(): Promise<void> {
    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.saving.set(true);

    try {
      const editing = this.editingInspection();
      if (editing) {
        await this.service.updateInspection(this.vehicleId, editing.id, this.formData);
      } else {
        await this.service.createInspection(this.vehicleId, this.formData);
      }
      this.closeModal();
      await this.loadInspections();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao salvar inspeção');
    } finally {
      this.saving.set(false);
    }
  }

  async deleteInspection(i: VehicleInspection): Promise<void> {
    if (!confirm(this.i18n.t('common.confirmDelete'))) return;

    try {
      await this.service.deleteInspection(this.vehicleId, i.id);
      await this.loadInspections();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao excluir inspeção');
    }
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'EUR' }).format(value);
  }

  getInspectionTypeLabel(type: InspectionType): string {
    return this.i18n.t(`vehicles.inspection.types.${InspectionType[type]}`) || InspectionType[type];
  }

  getInspectionResultLabel(result: InspectionResult): string {
    return this.i18n.t(`vehicles.inspection.results.${InspectionResult[result]}`) || InspectionResult[result];
  }

  getStatusBadge(inspection: VehicleInspection): { class: string; label: string } {
    if (inspection.result === InspectionResult.Rejected) {
      return { class: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300', label: this.i18n.t('vehicleDetail.inspections.failed') };
    }
    
    if (!inspection.expiryDate) {
      return { class: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300', label: this.i18n.t('vehicleDetail.inspections.passed') };
    }

    const expiry = new Date(inspection.expiryDate);
    const today = new Date();
    const daysUntilExpiry = Math.ceil((expiry.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));

    if (daysUntilExpiry < 0) {
      return { class: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300', label: this.i18n.t('common.expired') };
    }
    if (daysUntilExpiry <= 30) {
      return { class: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300', label: this.i18n.t('vehicleDetail.documents.expiringSoon') };
    }
    return { class: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300', label: this.i18n.t('common.valid') };
  }

  private getEmptyForm(): CreateInspectionRequest {
    const nextYear = new Date();
    nextYear.setFullYear(nextYear.getFullYear() + 1);
    return {
      type: InspectionType.ITV,
      inspectionDate: new Date().toISOString().split('T')[0],
      expiryDate: nextYear.toISOString().split('T')[0],
      result: InspectionResult.Pending,
      mileageAtInspection: 0,
      cost: 0
    };
  }
}
