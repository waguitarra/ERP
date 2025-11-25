import { Component, signal, output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DriversService, Driver } from '@core/services/drivers.service';

@Component({
  selector: 'app-driver-selector-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="isOpen()" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" (click)="close()">
      <div class="bg-white dark:bg-slate-900 rounded-xl shadow-2xl w-full max-w-5xl max-h-screen overflow-hidden mx-4" (click)="$event.stopPropagation()">
        
        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 flex justify-between items-center bg-green-600">
          <div class="flex items-center gap-3">
            <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
            </svg>
            <h2 class="text-2xl font-bold text-white">Selecionar Motorista</h2>
          </div>
          <button (click)="close()" class="text-white hover:bg-green-700 rounded-lg p-2">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>

        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800">
          <input 
            type="text" 
            [(ngModel)]="searchTerm"
            placeholder="🔍 Pesquisar por nome, CNH, telefone..."
            (ngModelChange)="searchTerm.set($event)"
            class="w-full px-4 py-3 border-2 border-slate-300 rounded-lg dark:bg-slate-700 dark:border-slate-600 dark:text-slate-100 focus:ring-2 focus:ring-green-500 focus:border-green-500 text-lg">
        </div>

        <div class="p-6 overflow-y-auto" style="max-height: 60vh;">
          <div *ngIf="loading()" class="text-center py-12">
            <div class="inline-block w-12 h-12 border-4 border-green-600 border-t-transparent rounded-full animate-spin"></div>
            <p class="mt-4 text-slate-600 dark:text-slate-400">Carregando motoristas...</p>
          </div>

          <div *ngIf="!loading() && filteredDrivers.length === 0" class="text-center py-12 text-slate-500">
            <svg class="w-20 h-20 mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
            </svg>
            <p class="text-xl">Nenhum motorista encontrado</p>
          </div>
          
          <div *ngIf="!loading() && filteredDrivers.length > 0" class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div 
              *ngFor="let driver of filteredDrivers"
              (click)="selectDriver(driver)"
              class="p-6 border-2 rounded-xl cursor-pointer transition-all duration-200 hover:shadow-lg"
              [class.border-green-600]="selectedDriver()?.id === driver.id"
              [class.bg-green-50]="selectedDriver()?.id === driver.id"
              [class.dark:bg-green-900]="selectedDriver()?.id === driver.id"
              [class.border-slate-300]="selectedDriver()?.id !== driver.id"
              [class.hover:border-green-400]="selectedDriver()?.id !== driver.id">
              
              <div class="flex items-start justify-between mb-4">
                <div class="flex-1">
                  <div class="text-2xl font-bold text-slate-900 dark:text-slate-100">
                    {{driver.name}}
                  </div>
                  <div class="text-sm text-slate-600 dark:text-slate-400 mt-1">
                    CNH: <span class="font-mono font-semibold">{{driver.licenseNumber}}</span>
                  </div>
                </div>
                <div *ngIf="driver.isActive" class="px-3 py-1 bg-green-100 text-green-800 rounded-full text-sm font-semibold">
                  ATIVO
                </div>
                <div *ngIf="!driver.isActive" class="px-3 py-1 bg-red-100 text-red-800 rounded-full text-sm font-semibold">
                  INATIVO
                </div>
              </div>
              
              <div class="space-y-3 text-sm">
                <div class="flex items-center gap-2">
                  <svg class="w-5 h-5 text-slate-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"></path>
                  </svg>
                  <span class="text-slate-800 dark:text-slate-200 font-medium">{{driver.phone}}</span>
                </div>
                <div *ngIf="driver.email" class="flex items-center gap-2">
                  <svg class="w-5 h-5 text-slate-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
                  </svg>
                  <span class="text-slate-800 dark:text-slate-200">{{driver.email}}</span>
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
            [disabled]="!selectedDriver()"
            class="px-6 py-3 bg-green-600 hover:bg-green-700 text-white rounded-lg transition disabled:opacity-50 disabled:cursor-not-allowed font-semibold">
            ✓ Confirmar Motorista
          </button>
        </div>

      </div>
    </div>
  `,
  styles: [`
    .animate-spin {
      animation: spin 1s linear infinite;
    }
    @keyframes spin {
      from { transform: rotate(0deg); }
      to { transform: rotate(360deg); }
    }
  `]
})
export class DriverSelectorModalComponent implements OnInit {
  private readonly driversService = inject(DriversService);
  
  driverSelected = output<Driver>();
  
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  searchTerm = signal<string>('');
  selectedDriver = signal<Driver | null>(null);
  drivers = signal<Driver[]>([]);
  
  get filteredDrivers(): Driver[] {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.drivers();
    
    return this.drivers().filter(driver => 
      driver.name.toLowerCase().includes(term) ||
      driver.licenseNumber.toLowerCase().includes(term) ||
      driver.phone.toLowerCase().includes(term) ||
      (driver.email && driver.email.toLowerCase().includes(term))
    );
  }

  ngOnInit(): void {
    this.loadDrivers();
  }

  async loadDrivers(): Promise<void> {
    this.loading.set(true);
    try {
      const drivers = await this.driversService.getAll();
      this.drivers.set(drivers);
    } catch (error) {
      console.error('Erro ao carregar motoristas:', error);
    } finally {
      this.loading.set(false);
    }
  }

  open(): void {
    this.isOpen.set(true);
    this.searchTerm.set('');
    this.selectedDriver.set(null);
    this.loadDrivers();
  }

  close(): void {
    this.isOpen.set(false);
  }

  selectDriver(driver: Driver): void {
    this.selectedDriver.set(driver);
  }

  confirm(): void {
    const selected = this.selectedDriver();
    if (selected) {
      this.driverSelected.emit(selected);
      this.close();
    }
  }
}
