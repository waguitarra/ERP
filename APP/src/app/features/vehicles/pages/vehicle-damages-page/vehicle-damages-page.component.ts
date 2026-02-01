import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../../vehicle-management.service';
import { VehiclesService, Vehicle } from '../../vehicles.service';
import { VehicleDamage } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-vehicle-damages-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './vehicle-damages-page.component.html'
})
export class VehicleDamagesPageComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly router = inject(Router);
  private readonly vehicleManagementService = inject(VehicleManagementService);
  private readonly vehiclesService = inject(VehiclesService);

  vehicles = signal<Vehicle[]>([]);
  selectedVehicleId = '';
  damages = signal<VehicleDamage[]>([]);
  allDamages = signal<VehicleDamage[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void { this.loadData(); }

  async loadData(): Promise<void> {
    this.loading.set(true);
    try {
      const vehiclesResponse = await this.vehiclesService.getAll();
      this.vehicles.set(vehiclesResponse.data || []);
      const damageResponse = await this.vehicleManagementService.getAllDamages();
      const allDmg = damageResponse.data || [];
      allDmg.sort((a, b) => new Date(b.occurrenceDate).getTime() - new Date(a.occurrenceDate).getTime());
      this.allDamages.set(allDmg);
      this.damages.set(allDmg);
    } catch (err) {
      this.error.set('Erro ao carregar avarias');
    } finally {
      this.loading.set(false);
    }
  }

  onVehicleChange(): void {
    const vehicleId = this.selectedVehicleId;
    this.damages.set(vehicleId ? this.allDamages().filter(d => d.vehicleId === vehicleId) : this.allDamages());
  }

  openVehicleDetail(vehicleId: string): void {
    this.router.navigate(['/vehicles', vehicleId], { queryParams: { tab: 'damages' } });
  }

  getSeverityClass(severity: string): string {
    const classes: Record<string, string> = {
      'Minor': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
      'Moderate': 'bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-400',
      'Severe': 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400',
      'Critical': 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
    };
    return classes[severity] || 'bg-slate-100 text-slate-800';
  }

  getSeverityLabel(severity: string): string {
    const labels: Record<string, string> = { 'Minor': 'Leve', 'Moderate': 'Moderado', 'Severe': 'Grave', 'Critical': 'Crítico' };
    return labels[severity] || severity;
  }

  getStatusClass(status: string): string {
    const classes: Record<string, string> = {
      'Reported': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
      'UnderRepair': 'bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-400',
      'Repaired': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
      'Closed': 'bg-slate-100 text-slate-800 dark:bg-slate-700 dark:text-slate-300'
    };
    return classes[status] || 'bg-slate-100 text-slate-800';
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = { 'Reported': 'Reportado', 'UnderRepair': 'Em Reparo', 'Repaired': 'Reparado', 'Closed': 'Fechado' };
    return labels[status] || status;
  }

  formatDate(date: string | Date | null): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number | null): string {
    if (value === null || value === undefined) return '-';
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }
}
