import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../vehicle-management.service';
import { VehicleMaintenance, MaintenanceType, CreateMaintenanceRequest } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-vehicle-maintenance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-maintenance.component.html',
  styleUrls: ['./vehicle-maintenance.component.scss']
})
export class VehicleMaintenanceComponent implements OnInit {
  @Input({ required: true }) vehicleId!: string;

  protected readonly i18n = inject(I18nService);
  private readonly service = inject(VehicleManagementService);

  maintenances = signal<VehicleMaintenance[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = signal(false);
  editingMaintenance = signal<VehicleMaintenance | null>(null);
  saving = signal(false);

  formData: CreateMaintenanceRequest = this.getEmptyForm();

  maintenanceTypes = Object.entries(MaintenanceType)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  ngOnInit(): void {
    this.loadMaintenances();
  }

  async loadMaintenances(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const response = await this.service.getMaintenances(this.vehicleId);
      this.maintenances.set(response.data || []);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar manutenções');
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void {
    this.formData = this.getEmptyForm();
    this.editingMaintenance.set(null);
    this.showModal.set(true);
  }

  openEditModal(m: VehicleMaintenance): void {
    this.formData = {
      type: m.type,
      description: m.description,
      maintenanceDate: m.maintenanceDate.split('T')[0],
      nextMaintenanceDate: m.nextMaintenanceDate?.split('T')[0],
      mileageAtMaintenance: m.mileageAtMaintenance,
      nextMaintenanceMileage: m.nextMaintenanceMileage,
      laborCost: m.laborCost,
      partsCost: m.partsCost,
      serviceProvider: m.serviceProvider,
      serviceProviderContact: m.serviceProviderContact,
      invoiceNumber: m.invoiceNumber,
      notes: m.notes
    };
    this.editingMaintenance.set(m);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingMaintenance.set(null);
  }

  async saveMaintenance(): Promise<void> {
    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.saving.set(true);

    try {
      const editing = this.editingMaintenance();
      if (editing) {
        await this.service.updateMaintenance(this.vehicleId, editing.id, this.formData);
      } else {
        await this.service.createMaintenance(this.vehicleId, this.formData);
      }
      this.closeModal();
      await this.loadMaintenances();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao salvar manutenção');
    } finally {
      this.saving.set(false);
    }
  }

  async deleteMaintenance(m: VehicleMaintenance): Promise<void> {
    if (!confirm(this.i18n.t('vehicleDetail.maintenance.confirmDelete'))) return;

    try {
      await this.service.deleteMaintenance(this.vehicleId, m.id);
      await this.loadMaintenances();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao excluir manutenção');
    }
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'EUR' }).format(value);
  }

  formatNumber(value: number): string {
    return new Intl.NumberFormat('pt-BR').format(value);
  }

  getMaintenanceTypeLabel(type: MaintenanceType): string {
    const typeKey = MaintenanceType[type]?.charAt(0).toLowerCase() + MaintenanceType[type]?.slice(1);
    return this.i18n.t(`vehicleDetail.maintenance.types.${typeKey}`) || MaintenanceType[type];
  }

  private getEmptyForm(): CreateMaintenanceRequest {
    return {
      type: MaintenanceType.Preventive,
      description: '',
      maintenanceDate: new Date().toISOString().split('T')[0],
      nextMaintenanceDate: undefined,
      mileageAtMaintenance: 0,
      nextMaintenanceMileage: undefined,
      laborCost: 0,
      partsCost: 0,
      serviceProvider: '',
      serviceProviderContact: '',
      invoiceNumber: '',
      notes: ''
    };
  }
}
