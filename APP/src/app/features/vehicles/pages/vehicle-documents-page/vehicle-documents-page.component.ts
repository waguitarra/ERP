import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@shared/pipes/translate.pipe';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../../vehicle-management.service';
import { VehiclesService, Vehicle } from '../../vehicles.service';
import { VehicleDocument } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-vehicle-documents-page',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './vehicle-documents-page.component.html'
})
export class VehicleDocumentsPageComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly router = inject(Router);
  private readonly vehicleManagementService = inject(VehicleManagementService);
  private readonly vehiclesService = inject(VehiclesService);

  vehicles = signal<Vehicle[]>([]);
  selectedVehicleId = '';
  documents = signal<VehicleDocument[]>([]);
  allDocuments = signal<VehicleDocument[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadData();
  }

  async loadData(): Promise<void> {
    this.loading.set(true);
    try {
      const vehiclesResponse = await this.vehiclesService.getAll();
      this.vehicles.set(vehiclesResponse.data || []);
      
      const allDocs: VehicleDocument[] = [];
      for (const vehicle of this.vehicles()) {
        try {
          const docsResponse = await this.vehicleManagementService.getDocuments(vehicle.id);
          if (docsResponse?.data) {
            allDocs.push(...docsResponse.data);
          }
        } catch (e) {}
      }
      
      this.allDocuments.set(allDocs);
      this.documents.set(allDocs);
    } catch (err) {
      this.error.set('Erro ao carregar documentos');
    } finally {
      this.loading.set(false);
    }
  }

  onVehicleChange(): void {
    const vehicleId = this.selectedVehicleId;
    if (vehicleId) {
      this.documents.set(this.allDocuments().filter(d => d.vehicleId === vehicleId));
    } else {
      this.documents.set(this.allDocuments());
    }
  }

  openVehicleDetail(vehicleId: string): void {
    this.router.navigate(['/vehicles', vehicleId], { queryParams: { tab: 'documents' } });
  }

  getDocumentTypeLabel(type: string): string {
    const types: Record<string, string> = {
      'Registration': 'Registro/CRLV', 'Insurance': 'Seguro', 'License': 'Licenciamento',
      'Inspection': 'Inspeção', 'Permit': 'Autorização', 'Contract': 'Contrato', 'Other': 'Outro'
    };
    return types[type] || type;
  }

  getStatusClass(doc: VehicleDocument): string {
    if (doc.isExpired) return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400';
    if (doc.isExpiringSoon) return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400';
    return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400';
  }

  getStatusLabel(doc: VehicleDocument): string {
    if (doc.isExpired) return 'Vencido';
    if (doc.isExpiringSoon) return 'Vence em breve';
    return 'Válido';
  }

  formatDate(date: string | Date | null | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }
}
