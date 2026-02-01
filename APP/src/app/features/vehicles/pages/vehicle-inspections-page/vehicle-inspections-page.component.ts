import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../../vehicle-management.service';
import { VehiclesService, Vehicle } from '../../vehicles.service';
import { VehicleInspection } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-vehicle-inspections-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './vehicle-inspections-page.component.html'
})
export class VehicleInspectionsPageComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly router = inject(Router);
  private readonly vehicleManagementService = inject(VehicleManagementService);
  private readonly vehiclesService = inject(VehiclesService);

  vehicles = signal<Vehicle[]>([]);
  selectedVehicleId = '';
  inspections = signal<VehicleInspection[]>([]);
  allInspections = signal<VehicleInspection[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void { this.loadData(); }

  async loadData(): Promise<void> {
    this.loading.set(true);
    try {
      const vehiclesResponse = await this.vehiclesService.getAll();
      this.vehicles.set(vehiclesResponse.data || []);
      
      const allInsp: VehicleInspection[] = [];
      for (const vehicle of this.vehicles()) {
        try {
          const response = await this.vehicleManagementService.getInspections(vehicle.id);
          if (response?.data) allInsp.push(...response.data);
        } catch (e) {}
      }
      
      allInsp.sort((a, b) => new Date(b.inspectionDate).getTime() - new Date(a.inspectionDate).getTime());
      this.allInspections.set(allInsp);
      this.inspections.set(allInsp);
    } catch (err) {
      this.error.set('Erro ao carregar inspeções');
    } finally {
      this.loading.set(false);
    }
  }

  onVehicleChange(): void {
    const vehicleId = this.selectedVehicleId;
    this.inspections.set(vehicleId ? this.allInspections().filter(i => i.vehicleId === vehicleId) : this.allInspections());
  }

  openVehicleDetail(vehicleId: string): void {
    this.router.navigate(['/vehicles', vehicleId], { queryParams: { tab: 'inspections' } });
  }

  getStatusClass(inspection: VehicleInspection): string {
    if (!inspection.expiryDate) return 'bg-slate-100 text-slate-800';
    const daysUntil = Math.ceil((new Date(inspection.expiryDate).getTime() - Date.now()) / 86400000);
    if (daysUntil < 0) return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400';
    if (daysUntil <= 30) return 'bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-400';
    return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400';
  }

  getStatusLabel(inspection: VehicleInspection): string {
    if (!inspection.expiryDate) return 'Sem validade';
    const daysUntil = Math.ceil((new Date(inspection.expiryDate).getTime() - Date.now()) / 86400000);
    if (daysUntil < 0) return 'Vencida';
    if (daysUntil <= 30) return `Vence em ${daysUntil}d`;
    return 'Válida';
  }

  formatDate(date: string | Date | null): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }
}
