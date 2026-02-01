import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../../vehicle-management.service';
import { VehiclesService, Vehicle } from '../../vehicles.service';
import { VehicleMaintenance } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-vehicle-maintenance-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './vehicle-maintenance-page.component.html'
})
export class VehicleMaintenancePageComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly router = inject(Router);
  private readonly vehicleManagementService = inject(VehicleManagementService);
  private readonly vehiclesService = inject(VehiclesService);

  vehicles = signal<Vehicle[]>([]);
  selectedVehicleId = '';
  maintenances = signal<VehicleMaintenance[]>([]);
  allMaintenances = signal<VehicleMaintenance[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void { this.loadData(); }

  async loadData(): Promise<void> {
    this.loading.set(true);
    try {
      const vehiclesResponse = await this.vehiclesService.getAll();
      this.vehicles.set(vehiclesResponse.data || []);
      
      const allMaint: VehicleMaintenance[] = [];
      for (const vehicle of this.vehicles()) {
        try {
          const response = await this.vehicleManagementService.getMaintenances(vehicle.id);
          if (response?.data) allMaint.push(...response.data);
        } catch (e) {}
      }
      
      allMaint.sort((a, b) => new Date(b.maintenanceDate).getTime() - new Date(a.maintenanceDate).getTime());
      this.allMaintenances.set(allMaint);
      this.maintenances.set(allMaint);
    } catch (err) {
      this.error.set('Erro ao carregar manutenções');
    } finally {
      this.loading.set(false);
    }
  }

  onVehicleChange(): void {
    const vehicleId = this.selectedVehicleId;
    this.maintenances.set(vehicleId ? this.allMaintenances().filter(m => m.vehicleId === vehicleId) : this.allMaintenances());
  }

  openVehicleDetail(vehicleId: string): void {
    this.router.navigate(['/vehicles', vehicleId], { queryParams: { tab: 'maintenance' } });
  }

  getMaintenanceTypeLabel(type: string): string {
    const types: Record<string, string> = { 'Preventive': 'Preventiva', 'Corrective': 'Corretiva', 'Scheduled': 'Programada', 'Emergency': 'Emergência', 'Recall': 'Recall', 'Other': 'Outra' };
    return types[type] || type;
  }

  formatDate(date: string | Date | null): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }

  formatNumber(value: number): string {
    return new Intl.NumberFormat('pt-BR').format(value);
  }
}
