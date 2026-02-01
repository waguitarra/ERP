import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../vehicle-management.service';
import { VehicleMileageLog, MileageLogType, MileageLogStatus, CreateMileageLogRequest, CompleteMileageLogRequest } from '@core/models/vehicle-management.model';

// Interface para o formulário que combina campos de criação e atualização
interface MileageLogFormData extends CreateMileageLogRequest {
  status?: MileageLogStatus;
  endMileage?: number;
  endDateTime?: string;
  endLatitude?: number;
  endLongitude?: number;
  endAddress?: string;
  fuelConsumed?: number;
  fuelCost?: number;
  tollCost?: number;
  parkingCost?: number;
  otherCosts?: number;
}

@Component({
  selector: 'app-vehicle-mileage',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-mileage.component.html',
  styleUrls: ['./vehicle-mileage.component.scss']
})
export class VehicleMileageComponent implements OnInit {
  @Input({ required: true }) vehicleId!: string;

  protected readonly i18n = inject(I18nService);
  private readonly service = inject(VehicleManagementService);

  mileageLogs = signal<VehicleMileageLog[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = signal(false);
  editingLog = signal<VehicleMileageLog | null>(null);
  saving = signal(false);

  formData: MileageLogFormData = this.getEmptyForm();

  // Stats
  totalDistance = signal(0);
  totalFuelCost = signal(0);
  avgFuelEfficiency = signal(0);

  logTypes = Object.entries(MileageLogType)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  logStatuses = Object.entries(MileageLogStatus)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  ngOnInit(): void {
    this.loadMileageLogs();
  }

  async loadMileageLogs(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const response = await this.service.getMileageLogs(this.vehicleId);
      const logs = response.data || [];
      this.mileageLogs.set(logs);
      this.calculateStats(logs);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar registros de quilometragem');
    } finally {
      this.loading.set(false);
    }
  }

  calculateStats(logs: VehicleMileageLog[]): void {
    const completedLogs = logs.filter(l => l.status === MileageLogStatus.Completed);
    
    const totalDist = completedLogs.reduce((sum, l) => sum + (l.distance || 0), 0);
    const totalFuel = completedLogs.reduce((sum, l) => sum + (l.fuelCost || 0), 0);
    
    this.totalDistance.set(totalDist);
    this.totalFuelCost.set(totalFuel);

    const logsWithEfficiency = completedLogs.filter(l => l.fuelEfficiency && l.fuelEfficiency > 0);
    if (logsWithEfficiency.length > 0) {
      const avgEff = logsWithEfficiency.reduce((sum, l) => sum + (l.fuelEfficiency || 0), 0) / logsWithEfficiency.length;
      this.avgFuelEfficiency.set(avgEff);
    } else {
      this.avgFuelEfficiency.set(0);
    }
  }

  openCreateModal(): void {
    this.formData = this.getEmptyForm();
    this.editingLog.set(null);
    this.showModal.set(true);
  }

  openEditModal(log: VehicleMileageLog): void {
    this.formData = {
      type: log.type,
      startMileage: log.startMileage,
      startDateTime: log.startDateTime.slice(0, 16),
      startLatitude: log.startLatitude,
      startLongitude: log.startLongitude,
      startAddress: log.startAddress,
      purpose: log.purpose
    };
    this.editingLog.set(log);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingLog.set(null);
  }

  async saveMileageLog(): Promise<void> {
    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.saving.set(true);

    try {
      const editing = this.editingLog();
      if (editing) {
        await this.service.updateMileageLog(this.vehicleId, editing.id, this.formData);
      } else {
        await this.service.createMileageLog(this.vehicleId, this.formData);
      }
      this.closeModal();
      await this.loadMileageLogs();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao salvar registro');
    } finally {
      this.saving.set(false);
    }
  }

  async completeTrip(log: VehicleMileageLog): Promise<void> {
    const endMileage = prompt(this.i18n.t('vehicleDetail.mileage.enterEndMileage'));
    if (!endMileage || isNaN(Number(endMileage))) return;

    try {
      const data: CompleteMileageLogRequest = {
        endMileage: Number(endMileage),
        endDateTime: new Date().toISOString()
      };
      await this.service.completeMileageLog(this.vehicleId, log.id, data);
      await this.loadMileageLogs();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao finalizar viagem');
    }
  }

  async deleteMileageLog(log: VehicleMileageLog): Promise<void> {
    if (!confirm(this.i18n.t('common.confirmDelete'))) return;

    try {
      await this.service.deleteMileageLog(this.vehicleId, log.id);
      await this.loadMileageLogs();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao excluir registro');
    }
  }

  formatDateTime(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleString('pt-BR', { 
      dateStyle: 'short', 
      timeStyle: 'short' 
    });
  }

  formatNumber(value: number | undefined): string {
    if (value === undefined) return '-';
    return new Intl.NumberFormat('pt-BR').format(value);
  }

  formatCurrency(value: number | undefined): string {
    if (value === undefined) return '-';
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'EUR' }).format(value);
  }

  formatDuration(minutes: number | undefined): string {
    if (minutes === undefined) return '-';
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    if (hours > 0) {
      return `${hours}h ${mins}m`;
    }
    return `${mins}m`;
  }

  getLogTypeLabel(type: MileageLogType): string {
    return this.i18n.t(`vehicleDetail.mileage.types.${MileageLogType[type]?.toLowerCase()}`) || MileageLogType[type];
  }

  getStatusLabel(status: MileageLogStatus): string {
    return this.i18n.t(`vehicleDetail.mileage.status.${MileageLogStatus[status]?.toLowerCase()}`) || MileageLogStatus[status];
  }

  getStatusBadge(status: MileageLogStatus): string {
    const badges: Record<number, string> = {
      [MileageLogStatus.Planned]: 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-300',
      [MileageLogStatus.InProgress]: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
      [MileageLogStatus.Completed]: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
      [MileageLogStatus.Cancelled]: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    };
    return badges[status] || 'bg-gray-100 text-gray-800';
  }

  private getEmptyForm(): MileageLogFormData {
    return {
      type: MileageLogType.Delivery,
      startMileage: 0,
      startDateTime: new Date().toISOString().slice(0, 16),
      status: MileageLogStatus.InProgress
    };
  }
}
