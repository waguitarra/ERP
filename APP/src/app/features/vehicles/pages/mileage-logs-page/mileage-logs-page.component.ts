import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../../vehicle-management.service';
import { VehiclesService, Vehicle } from '../../vehicles.service';
import { VehicleMileageLog } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-mileage-logs-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './mileage-logs-page.component.html'
})
export class MileageLogsPageComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly router = inject(Router);
  private readonly vehicleManagementService = inject(VehicleManagementService);
  private readonly vehiclesService = inject(VehiclesService);

  vehicles = signal<Vehicle[]>([]);
  selectedVehicleId = '';
  mileageLogs = signal<VehicleMileageLog[]>([]);
  allMileageLogs = signal<VehicleMileageLog[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  totalDistance = signal(0);
  totalFuelCost = signal(0);
  totalTollCost = signal(0);

  ngOnInit(): void { this.loadData(); }

  async loadData(): Promise<void> {
    this.loading.set(true);
    try {
      const vehiclesResponse = await this.vehiclesService.getAll();
      this.vehicles.set(vehiclesResponse.data || []);
      
      const allLogs: VehicleMileageLog[] = [];
      for (const vehicle of this.vehicles()) {
        try {
          const response = await this.vehicleManagementService.getMileageLogs(vehicle.id);
          if (response?.data) allLogs.push(...response.data);
        } catch (e) {}
      }
      
      allLogs.sort((a, b) => new Date(b.startDateTime).getTime() - new Date(a.startDateTime).getTime());
      this.allMileageLogs.set(allLogs);
      this.mileageLogs.set(allLogs);
      this.calculateTotals(allLogs);
    } catch (err) {
      this.error.set('Erro ao carregar registros de quilometragem');
    } finally {
      this.loading.set(false);
    }
  }

  calculateTotals(logs: VehicleMileageLog[]): void {
    this.totalDistance.set(logs.reduce((sum, l) => sum + l.distance, 0));
    this.totalFuelCost.set(logs.reduce((sum, l) => sum + (l.fuelCost || 0), 0));
    this.totalTollCost.set(logs.reduce((sum, l) => sum + (l.tollCost || 0), 0));
  }

  onVehicleChange(): void {
    const vehicleId = this.selectedVehicleId;
    const filtered = vehicleId ? this.allMileageLogs().filter(l => l.vehicleId === vehicleId) : this.allMileageLogs();
    this.mileageLogs.set(filtered);
    this.calculateTotals(filtered);
  }

  openVehicleDetail(vehicleId: string): void {
    this.router.navigate(['/vehicles', vehicleId], { queryParams: { tab: 'mileage' } });
  }

  formatDate(date: string | Date | null): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatNumber(value: number): string {
    return new Intl.NumberFormat('pt-BR').format(value);
  }

  formatCurrency(value: number | null | undefined): string {
    if (value === null || value === undefined) return '-';
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  }
}
