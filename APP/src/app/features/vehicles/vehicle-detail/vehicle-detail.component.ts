import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { I18nService } from '@core/services/i18n.service';
import { VehiclesService } from '../vehicles.service';
import { VehicleManagementService } from '../vehicle-management.service';
import { Vehicle } from '@core/models/vehicle.model';
import { VehicleSummary, VehicleAlert } from '@core/models/vehicle-management.model';

// Sub-components
import { VehicleDocumentsComponent } from '../vehicle-documents/vehicle-documents.component';
import { VehicleMaintenanceComponent } from '../vehicle-maintenance/vehicle-maintenance.component';
import { VehicleInspectionsComponent } from '../vehicle-inspections/vehicle-inspections.component';
import { VehicleDamagesComponent } from '../vehicle-damages/vehicle-damages.component';
import { VehicleMileageComponent } from '../vehicle-mileage/vehicle-mileage.component';

type TabType = 'overview' | 'documents' | 'maintenance' | 'inspections' | 'damages' | 'mileage';

@Component({
  selector: 'app-vehicle-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    VehicleDocumentsComponent,
    VehicleMaintenanceComponent,
    VehicleInspectionsComponent,
    VehicleDamagesComponent,
    VehicleMileageComponent
  ],
  templateUrl: './vehicle-detail.component.html',
  styleUrls: ['./vehicle-detail.component.scss']
})
export class VehicleDetailComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly vehiclesService = inject(VehiclesService);
  private readonly managementService = inject(VehicleManagementService);

  vehicleId = signal<string>('');
  vehicle = signal<Vehicle | null>(null);
  summary = signal<VehicleSummary | null>(null);
  alerts = signal<VehicleAlert[]>([]);
  
  activeTab = signal<TabType>('overview');
  loading = signal(true);
  error = signal<string | null>(null);

  criticalAlerts = computed(() => 
    this.alerts().filter(a => a.severity === 'Critical' || a.severity === 'High')
  );

  tabs: { id: TabType; labelKey: string; icon: string }[] = [
    { id: 'overview', labelKey: 'vehicleDetail.tabs.overview', icon: 'dashboard' },
    { id: 'documents', labelKey: 'vehicleDetail.tabs.documents', icon: 'description' },
    { id: 'maintenance', labelKey: 'vehicleDetail.tabs.maintenance', icon: 'build' },
    { id: 'inspections', labelKey: 'vehicleDetail.tabs.inspections', icon: 'verified' },
    { id: 'damages', labelKey: 'vehicleDetail.tabs.damages', icon: 'report_problem' },
    { id: 'mileage', labelKey: 'vehicleDetail.tabs.mileage', icon: 'speed' }
  ];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    const tab = this.route.snapshot.queryParamMap.get('tab') as TabType;
    
    if (id) {
      this.vehicleId.set(id);
      if (tab && this.tabs.some(t => t.id === tab)) {
        this.activeTab.set(tab);
      }
      this.loadVehicle();
    } else {
      this.router.navigate(['/vehicles']);
    }
  }

  async loadVehicle(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const [vehicleResponse, summaryResponse, alertsResponse] = await Promise.all([
        this.vehiclesService.getById(this.vehicleId()),
        this.managementService.getVehicleSummary(this.vehicleId()),
        this.managementService.getVehicleAlerts(this.vehicleId())
      ]);

      this.vehicle.set(vehicleResponse.data);
      this.summary.set(summaryResponse.data);
      this.alerts.set(alertsResponse.data || []);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar dados do veículo');
      console.error('Error loading vehicle:', err);
    } finally {
      this.loading.set(false);
    }
  }

  setActiveTab(tab: TabType): void {
    this.activeTab.set(tab);
  }

  goBack(): void {
    this.router.navigate(['/vehicles']);
  }

  getStatusClass(status: number): string {
    const classes: Record<number, string> = {
      0: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
      1: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
      2: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
      3: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    };
    return classes[status] || 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300';
  }

  getStatusLabel(status: number): string {
    const labels: Record<number, string> = {
      0: 'available',
      1: 'inTransit',
      2: 'maintenance',
      3: 'inactive'
    };
    return this.i18n.t(`vehicles.status.${labels[status] || 'available'}`);
  }

  getAlertSeverityClass(severity: string): string {
    const classes: Record<string, string> = {
      'Critical': 'bg-red-100 text-red-800 border-red-300 dark:bg-red-900/50 dark:text-red-300',
      'High': 'bg-orange-100 text-orange-800 border-orange-300 dark:bg-orange-900/50 dark:text-orange-300',
      'Medium': 'bg-yellow-100 text-yellow-800 border-yellow-300 dark:bg-yellow-900/50 dark:text-yellow-300',
      'Low': 'bg-blue-100 text-blue-800 border-blue-300 dark:bg-blue-900/50 dark:text-blue-300'
    };
    return classes[severity] || classes['Medium'];
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'EUR' }).format(value);
  }

  formatNumber(value: number): string {
    return new Intl.NumberFormat('pt-BR').format(value);
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }
}
