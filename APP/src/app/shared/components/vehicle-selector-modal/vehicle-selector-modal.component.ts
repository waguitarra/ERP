import { Component, signal, output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VehiclesService, Vehicle } from '@core/services/vehicles.service';

@Component({
  selector: 'app-vehicle-selector-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="isOpen()" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" (click)="close()">
      <div class="bg-white dark:bg-slate-900 rounded-xl shadow-2xl w-full max-w-5xl max-h-screen overflow-hidden mx-4" (click)="$event.stopPropagation()">
        
        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 flex justify-between items-center bg-blue-600">
          <div class="flex items-center gap-3">
            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <h2 class="text-2xl font-bold text-white">Selecionar Veículo</h2>
          </div>
          <button (click)="close()" class="text-white hover:bg-blue-700 rounded-lg p-2">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>

        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800">
          <input 
            type="text" 
            [(ngModel)]="searchTerm"
            placeholder="🔍 Pesquisar por placa, modelo, cor..."
            (ngModelChange)="searchTerm.set($event)"
            class="w-full px-4 py-3 border-2 border-slate-300 rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-slate-100 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-lg">
        </div>

        <div class="p-6 overflow-y-auto" style="max-height: 60vh;">
          <div *ngIf="loading()" class="text-center py-12">
            <div class="inline-block w-12 h-12 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
            <p class="mt-4 text-slate-600 dark:text-slate-400">Carregando veículos...</p>
          </div>

          <div *ngIf="!loading() && filteredVehicles.length === 0" class="text-center py-12 text-slate-500">
            <svg class="w-20 h-20 mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <p class="text-xl">Nenhum veículo encontrado</p>
          </div>
          
          <div *ngIf="!loading() && filteredVehicles.length > 0" class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div 
              *ngFor="let vehicle of filteredVehicles"
              (click)="selectVehicle(vehicle)"
              class="p-6 border-2 rounded-xl cursor-pointer transition-all duration-200 hover:shadow-lg"
              [class.border-blue-600]="selectedVehicle()?.id === vehicle.id"
              [class.bg-blue-50]="selectedVehicle()?.id === vehicle.id"
              [class.dark:bg-blue-900]="selectedVehicle()?.id === vehicle.id"
              [class.border-slate-300]="selectedVehicle()?.id !== vehicle.id"
              [class.hover:border-blue-400]="selectedVehicle()?.id !== vehicle.id">
              
              <div class="flex items-start justify-between mb-4">
                <div>
                  <div class="text-2xl font-bold text-slate-900 dark:text-slate-100">
                    {{vehicle.licensePlate}}
                  </div>
                  <div class="text-lg text-slate-600 dark:text-slate-400">
                    {{vehicle.model}}
                  </div>
                </div>
                <div *ngIf="vehicle.isActive" class="px-3 py-1 bg-green-100 text-green-800 rounded-full text-sm font-semibold">
                  ATIVO
                </div>
                <div *ngIf="!vehicle.isActive" class="px-3 py-1 bg-red-100 text-red-800 rounded-full text-sm font-semibold">
                  INATIVO
                </div>
              </div>
              
              <div class="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <div class="text-slate-500 dark:text-slate-400 font-semibold uppercase text-xs mb-1">Ano</div>
                  <div class="text-slate-800 dark:text-slate-200 font-medium">{{vehicle.year || '-'}}</div>
                </div>
                <div>
                  <div class="text-slate-500 dark:text-slate-400 font-semibold uppercase text-xs mb-1">Cor</div>
                  <div class="text-slate-800 dark:text-slate-200 font-medium">{{vehicle.color || '-'}}</div>
                </div>
                <div class="col-span-2">
                  <div class="text-slate-500 dark:text-slate-400 font-semibold uppercase text-xs mb-1">Capacidade</div>
                  <div class="text-slate-800 dark:text-slate-200 font-medium">{{vehicle.capacity || '-'}}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex justify-end gap-3 bg-slate-50 dark:bg-slate-800">
          <button 
            type="button" 
            (click)="close()"
            class="px-6 py-3 text-slate-700 dark:text-slate-300 hover:bg-slate-200 dark:hover:bg-slate-700 rounded-lg transition font-semibold">
            Cancelar
          </button>
          <button 
            type="button" 
            (click)="confirm()" 
            [disabled]="!selectedVehicle()"
            class="px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed font-semibold">
            ✓ Confirmar Veículo
          </button>
        </div>

      </div>
    </div>
  `,
  styles: [`
    @keyframes fadeIn {
      from { opacity: 0; }
      to { opacity: 1; }
    }
    .animate-spin {
      animation: spin 1s linear infinite;
    }
    @keyframes spin {
      from { transform: rotate(0deg); }
      to { transform: rotate(360deg); }
    }
  `]
})
export class VehicleSelectorModalComponent implements OnInit {
  private readonly vehiclesService = inject(VehiclesService);
  
  vehicleSelected = output<Vehicle>();
  
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  searchTerm = signal<string>('');
  selectedVehicle = signal<Vehicle | null>(null);
  vehicles = signal<Vehicle[]>([]);
  
  get filteredVehicles(): Vehicle[] {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.vehicles();
    
    return this.vehicles().filter(vehicle => 
      vehicle.licensePlate.toLowerCase().includes(term) ||
      vehicle.model.toLowerCase().includes(term) ||
      (vehicle.color && vehicle.color.toLowerCase().includes(term)) ||
      (vehicle.year && vehicle.year.toString().includes(term))
    );
  }

  ngOnInit(): void {
    this.loadVehicles();
  }

  async loadVehicles(): Promise<void> {
    this.loading.set(true);
    try {
      const vehicles = await this.vehiclesService.getAll();
      this.vehicles.set(vehicles);
    } catch (error) {
      console.error('Erro ao carregar veículos:', error);
    } finally {
      this.loading.set(false);
    }
  }

  open(): void {
    this.isOpen.set(true);
    this.searchTerm.set('');
    this.selectedVehicle.set(null);
    this.loadVehicles();
  }

  close(): void {
    this.isOpen.set(false);
  }

  selectVehicle(vehicle: Vehicle): void {
    this.selectedVehicle.set(vehicle);
  }

  confirm(): void {
    const selected = this.selectedVehicle();
    if (selected) {
      this.vehicleSelected.emit(selected);
      this.close();
    }
  }
}
